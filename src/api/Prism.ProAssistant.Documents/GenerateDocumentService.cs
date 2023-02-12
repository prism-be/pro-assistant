// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocumentService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Acme.Core.Extensions;
using DotLiquid;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Extensions;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace Prism.ProAssistant.Documents;

public interface IGenerateDocumentService
{
    Task<byte[]> Generate(string id, string appointmentId);
}

public class GenerateDocumentService : IGenerateDocumentService
{
    private readonly IFindManyService _findManyService;
    private readonly IFindOneService _findOneService;
    private readonly ILocalizator _localizator;
    private readonly ILogger<GenerateDocumentService> _logger;
    private readonly IOrganizationContext _organizationContext;

    public GenerateDocumentService(ILogger<GenerateDocumentService> logger, ILocalizator localizator, IOrganizationContext organizationContext, IFindOneService findOneService,
        IFindManyService findManyService)
    {
        _logger = logger;
        _localizator = localizator;
        _organizationContext = organizationContext;
        _findOneService = findOneService;
        _findManyService = findManyService;
    }

    public async Task<byte[]> Generate(string id, string appointmentId)
    {
        var data = await GetData(appointmentId);

        if (data == null)
        {
            throw new NotSupportedException("The document cannot be generated, check logs.");
        }

        var (title, content) = await GetTitleContent(id);

        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);

        var document = CreateDocument(data, title, content);
        var bytes = document.GeneratePdf();

        var computedTitle = ReplaceContent(title, data.Value.appointment, data.Value.contact, data.Value.settings);
        await SaveDocument(data.Value.appointment, computedTitle, bytes);

        return bytes;
    }

    private Document CreateDocument([DisallowNull] (Appointment appointment, Contact contact, Dictionary<string, Setting> settings)? data, string title, string content)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Table(table =>
                {
                    table.WriteHeader(data.Value.settings);
                    table.WriteContactAddress(data.Value.contact);

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(13, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(ReplaceContent(title, data.Value.appointment, data.Value.contact, data.Value.settings)).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(ReplaceContent(content, data.Value.appointment, data.Value.contact, data.Value.settings));
                    });

                    table.WriteSignature(data.Value.settings);
                });
            });
        });
    }

    private async Task<(Appointment appointment, Contact contact, Dictionary<string, Setting> settings)?> GetData(string appointmentId)
    {
        var appointment = await _findOneService.Find<Appointment>(appointmentId);

        if (appointment == null)
        {
            _logger.LogWarning("Cannot find Appointment {itemId}", appointmentId);
            return null;
        }

        if (string.IsNullOrWhiteSpace(appointment.ContactId))
        {
            _logger.LogWarning("Cannot find contact for Appointment {itemId}", appointmentId);
            return null;
        }

        var contact = await _findOneService.Find<Contact>(appointment.ContactId);

        if (contact == null)
        {
            _logger.LogWarning("Cannot find contact {itemId}", appointment.ContactId);
            return null;
        }

        var settings = await _findManyService.Find<Setting>();

        return (appointment, contact, settings.ToDictionary(x => x.Id));
    }

    private async Task<(string title, string content)> GetTitleContent(string documentId)
    {
        var document = await _findOneService.Find<DocumentConfiguration>(documentId);

        if (document == null)
        {
            throw new NotFoundException($"The document with id {documentId} cannot be found");
        }

        return (document.Title ?? string.Empty, document.Body ?? string.Empty);
    }

    private string ReplaceContent(string templateContent, Appointment appointment, Contact contact, Dictionary<string, Setting> settings)
    {
        var template = Template.Parse(templateContent);

        var data = new
        {
            name = settings["document-header-your-name"].Value,
            contactName = (contact.Title + " " + contact.LastName + " " + contact.FirstName).Trim(),
            price = appointment.Price.ToString("F2") + "€",
            appointmentType = appointment.Type,
            appointmentDate = appointment.StartDate.ToLongDateString(),
            appointmentHour = appointment.StartDate.ToShortTimeString(),
            paymentDate = (appointment.PaymentDate ?? appointment.StartDate).ToString("dd/MM/yyyy"),
            paymentMode = _localizator.GetTranslation("documents", "payment" + appointment.Payment)
        };

        return template.Render(Hash.FromAnonymousObject(data));
    }

    private async Task SaveDocument(Appointment appointment, string title, byte[] bytes)
    {
        var collection = _organizationContext.GetCollection<Appointment>();
        var existings = await collection.FindAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointment.Id));
        var existing = await existings.FirstAsync();

        var documentTitle = $"{appointment.StartDate:yyyy-MM-dd HH:mm} - {appointment.LastName} {appointment.FirstName} - {title}";
        var fileName = documentTitle.ReplaceSpecialChars(true) + ".pdf";

        var bucket = _organizationContext.GetGridFsBucket();
        var fileId = await bucket.UploadFromBytesAsync(fileName, bytes);

        var document = new BinaryDocument
        {
            Id = fileId.ToString(),
            Title = documentTitle,
            Date = DateTime.UtcNow,
            FileName = fileName
        };

        existing.Documents.Insert(0, document);

        await collection.UpdateOneAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointment.Id), Builders<Appointment>.Update.Set(x => x.Documents, existing.Documents));

        _logger.LogInformation("Document {itemId} was saved for Appointment {appointmentId}", document.Id, appointment.Id);
    }
}