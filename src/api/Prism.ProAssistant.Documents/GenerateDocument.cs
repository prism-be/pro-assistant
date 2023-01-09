// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Nodes;
using Acme.Core.Extensions;
using DotLiquid;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Extensions;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Document = QuestPDF.Fluent.Document;
using Unit = QuestPDF.Infrastructure.Unit;

namespace Prism.ProAssistant.Documents;

public record GenerateDocument(string DocumentId, string AppointmentId) : IRequest<byte[]>;

public class GenerateDocumentHandler : IRequestHandler<GenerateDocument, byte[]>
{
    private readonly ILocalizator _localizator;
    private readonly ILogger<GenerateDocumentHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IOrganizationContext _organizationContext;

    public GenerateDocumentHandler(IMediator mediator, ILogger<GenerateDocumentHandler> logger, ILocalizator localizator, IOrganizationContext organizationContext)
    {
        _mediator = mediator;
        _logger = logger;
        _localizator = localizator;
        _organizationContext = organizationContext;
    }

    public async Task<byte[]> Handle(GenerateDocument request, CancellationToken cancellationToken)
    {
        var data = await GetData(request.AppointmentId);

        if (data == null)
        {
            throw new NotSupportedException("The document cannot be generated, check logs.");
        }

        var (title, content) = await GetTitleContent(request.DocumentId);

        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);

        var document = CreateDocument(data, title, content);
        var bytes = document.GeneratePdf();

        var computedTitle = ReplaceContent(title, data.Value.appointment, data.Value.contact, data.Value.headers);
        await SaveDocument(data.Value.appointment, computedTitle, bytes);

        return bytes;
    }

    private Document CreateDocument([DisallowNull] (Appointment appointment, Contact contact, Setting setting, JsonNode headers)? data, string title, string content)
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
                    table.WriteHeader(data.Value.headers);
                    table.WriteContactAddress(data.Value.contact);

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(13, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(ReplaceContent(title, data.Value.appointment, data.Value.contact, data.Value.headers)).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(ReplaceContent(content, data.Value.appointment, data.Value.contact, data.Value.headers));
                    });

                    table.WriteSignature(data.Value.headers);
                });
            });
        });
    }

    private async Task<(Appointment appointment, Contact contact, Setting setting, JsonNode headers)?> GetData(string appointmentId)
    {
        var appointment = await _mediator.Send(new FindOne<Appointment>(appointmentId));

        if (appointment == null)
        {
            _logger.LogWarning("Cannot find Appointment {AppointmentId}", appointmentId);
            return null;
        }

        if (string.IsNullOrWhiteSpace(appointment.ContactId))
        {
            _logger.LogWarning("Cannot find contact for Appointment {AppointmentId}", appointmentId);
            return null;
        }

        var contact = await _mediator.Send(new FindOne<Contact>(appointment.ContactId));

        if (contact == null)
        {
            _logger.LogWarning("Cannot find contact {contactId}", appointment.ContactId);
            return null;
        }

        var setting = await _mediator.Send(new FindOne<Setting>("documents-headers"));

        if (setting == null || setting.Value == null)
        {
            _logger.LogWarning("Cannot find setting {settingId}", "documents-headers");
            return null;
        }

        var headers = JsonNode.Parse(setting.Value);

        if (headers == null)
        {
            _logger.LogWarning("Setting value is not a valid JSON");
            return null;
        }

        return (appointment, contact, setting, headers);
    }

    private async Task<(string title, string content)> GetTitleContent(string documentId)
    {
        var document = await _mediator.Send(new FindOne<DocumentConfiguration>(documentId));

        if (document == null)
        {
            throw new NotFoundException($"The document with id {documentId} cannot be found");
        }

        return (document.Title ?? string.Empty, document.Body ?? string.Empty);
    }

    private string ReplaceContent(string templateContent, Appointment appointment, Contact contact, JsonNode headers)
    {
        var template = Template.Parse(templateContent);

        var data = new
        {
            name = headers["yourName"]?.ToString(),
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

        _logger.LogInformation("Document {DocumentId} was saved for Appointment {AppointmentId}", document.Id, appointment.Id);
    }
}