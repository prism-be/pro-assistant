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
using ImageMagick;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

        var computedTitle = ReplaceContent(title, data.Value.appointment, data.Value.patient, data.Value.headers);
        await SaveDocument(data.Value.appointment, computedTitle, bytes);

        return bytes;
    }

    private async Task SaveDocument(Appointment appointment, string title, byte[] bytes)
    {
        var collection = _organizationContext.GetCollection<Appointment>();
        var existings = await collection.FindAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointment.Id));
        var existing = await existings.FirstAsync();

        var documentTitle = $"{appointment.StartDate:yyyy-MM-dd HH:mm} - {appointment.LastName} {appointment.FirstName} - {title}";
        string fileName = documentTitle.ReplaceSpecialChars(true) + ".pdf";
        
        var bucket = _organizationContext.GetGridFsBucket();
        var fileId = await bucket.UploadFromBytesAsync(fileName, bytes);

        var document = new BinaryDocument
        {
            Id = fileId.ToString(),
            Title = documentTitle,
            Date = DateTime.UtcNow,
            FileName = fileName,
        };
        
        existing.Documents.Insert(0,document);
        
        await collection.UpdateOneAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointment.Id), Builders<Appointment>.Update.Set(x => x.Documents, existing.Documents));
        
        _logger.LogInformation("Document {DocumentId} was saved for Appointment {AppointmentId}", document.Id, appointment.Id);
    }

    private Document CreateDocument([DisallowNull] (Appointment appointment, Patient patient, Setting setting, JsonNode headers)? data, string title, string content)
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
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(2, Unit.Centimetre);
                        columns.ConstantColumn(8, Unit.Centimetre);
                        columns.RelativeColumn();
                    });

                    var logo = data.Value.headers["logo"]?.ToString().Split(',').LastOrDefault();

                    if (logo != null)
                    {
                        var logoBytes = Convert.FromBase64String(logo);
                        logoBytes = ResizeImage(logoBytes, 250, 250);
                        table.Cell().Row(1).Column(1).Element(e => e.Height(2, Unit.Centimetre)).Image(logoBytes);
                    }

                    table.Cell().Row(1).Column(2).ColumnSpan(2).PaddingLeft(0.5f, Unit.Centimetre).Column(c =>
                    {
                        c.Item().Text(data.Value.headers["name"]?.ToString()).FontSize(10);

                        foreach (var line in data.Value.headers["address"]?.ToString().Split('\n') ?? Array.Empty<string>())
                        {
                            c.Item().Text(line).FontSize(10);
                        }
                    });

                    table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingTop(0.5f, Unit.Centimetre).Element(e => e.Height(0.25f, Unit.Centimetre)).LineHorizontal(1);

                    table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(DateTime.Today.ToLongDateString()).FontSize(10).LineHeight(0.75f);

                    table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(data.Value.patient.LastName + " " + data.Value.patient.FirstName);
                        c.Item().Text(data.Value.patient.Street + " " + data.Value.patient.Number);
                        c.Item().Text(data.Value.patient.ZipCode + " " + data.Value.patient.City);
                        c.Item().Text(data.Value.patient.Country);
                    });

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(13, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(ReplaceContent(title, data.Value.appointment, data.Value.patient, data.Value.headers)).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(ReplaceContent(content, data.Value.appointment, data.Value.patient, data.Value.headers));
                    });

                    table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
                    {
                        var signature = data.Value.headers["signature"]?.ToString().Split(',').LastOrDefault();

                        if (signature != null)
                        {
                            var signatureBytes = Convert.FromBase64String(signature);
                            signatureBytes = ResizeImage(signatureBytes, 400, 200);
                            c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(signatureBytes, ImageScaling.FitHeight);
                        }

                        c.Item().AlignRight().Text(data.Value.headers["yourName"]?.ToString());
                        c.Item().AlignRight().Text(data.Value.headers["yourCity"] + ", " + DateTime.Today.ToLongDateString()).FontSize(10);
                    });
                });
            });
        });
    }

    private byte[] ResizeImage(byte[] data,int width, int height)
    {
        var image = new MagickImage(data);
        
        if (image.Height < height && image.Width < width)
        {
            return data;
        }
        
        image.Resize(width, height);
        return image.ToByteArray();
    }

    private async Task<(Appointment appointment, Patient patient, Setting setting, JsonNode headers)?> GetData(string appointmentId)
    {
        var appointment = await _mediator.Send(new FindOne<Appointment>(appointmentId));

        if (appointment == null)
        {
            _logger.LogWarning("Cannot find Appointment {AppointmentId}", appointmentId);
            return null;
        }

        if (string.IsNullOrWhiteSpace(appointment.PatientId))
        {
            _logger.LogWarning("Cannot find patient for Appointment {AppointmentId}", appointmentId);
            return null;
        }

        var patient = await _mediator.Send(new FindOne<Patient>(appointment.PatientId));

        if (patient == null)
        {
            _logger.LogWarning("Cannot find patient {patientId}", appointment.PatientId);
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

        return (appointment, patient, setting, headers);
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

    private string ReplaceContent(string templateContent, Appointment appointment, Patient patient, JsonNode headers)
    {
        var template = Template.Parse(templateContent);

        var data = new
        {
            name = headers["yourName"]?.ToString(),
            patientName = (patient.Title + " " + patient.LastName + " " + patient.FirstName).Trim(),
            price = appointment.Price.ToString("F2") + "€",
            appointmentType = appointment.Type,
            appointmentDate = appointment.StartDate.ToLongDateString(),
            appointmentHour = appointment.StartDate.ToShortTimeString(),
            paymentDate = (appointment.PaymentDate ?? appointment.StartDate).ToString("dd/MM/yyyy"),
            paymentMode = _localizator.GetTranslation("documents", "payment" + appointment.Payment)
        };

        return template.Render(Hash.FromAnonymousObject(data));
    }
}