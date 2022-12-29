// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Nodes;
using DotLiquid;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using Unit = QuestPDF.Infrastructure.Unit;

namespace Prism.ProAssistant.Documents;

public record GenerateDocument(string DocumentId, string MeetingId) : IRequest<byte[]>;

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
        var data = await GetData(request.MeetingId);

        if (data == null)
        {
            throw new NotSupportedException("The document cannot be generated, check logs.");
        }

        var (title, content) = await GetTitleContent(request.DocumentId);

        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);

        var document = CreateDocument(data, title, content);
        var bytes = document.GeneratePdf();

        await SaveDocument(data.Value.meeting, title, bytes);

        return bytes;
    }

    private async Task SaveDocument(Meeting meeting, string title, byte[] bytes)
    {
        var collection = _organizationContext.GetCollection<Meeting>();
        var existings = await collection.FindAsync(Builders<Meeting>.Filter.Eq(x => x.Id, meeting.Id));
        var existing = await existings.FirstAsync();

        string fileName = Identifier.GenerateString() + ".pdf";
        var bucket = _organizationContext.GetGridFsBucket();
        var fileId = await bucket.UploadFromBytesAsync(fileName, bytes);

        var document = new BinaryDocument
        {
            Id = fileId.ToString(),
            Title = title,
            Date = DateTime.UtcNow,
            FileName = fileName,
        };
        
        existing.Documents.Insert(0,document);
        
        await collection.UpdateOneAsync(Builders<Meeting>.Filter.Eq(x => x.Id, meeting.Id), Builders<Meeting>.Update.Set(x => x.Documents, existing.Documents));
        
        _logger.LogInformation("Document {DocumentId} was saved for meeting {MeetingId}", document.Id, meeting.Id);
    }

    private Document CreateDocument([DisallowNull] (Meeting meeting, Patient patient, Setting setting, JsonNode headers)? data, string title, string content)
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

                    table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(data.Value.meeting.StartDate.ToLongDateString()).FontSize(10).LineHeight(0.75f);

                    table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(data.Value.patient.LastName + " " + data.Value.patient.FirstName);
                        c.Item().Text(data.Value.patient.Street + " " + data.Value.patient.Number);
                        c.Item().Text(data.Value.patient.ZipCode + " " + data.Value.patient.City);
                        c.Item().Text(data.Value.patient.Country);
                    });

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(13, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(ReplaceContent(title, data.Value.meeting, data.Value.patient, data.Value.headers)).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(ReplaceContent(content, data.Value.meeting, data.Value.patient, data.Value.headers));
                    });

                    table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
                    {
                        var signature = data.Value.headers["signature"]?.ToString().Split(',').LastOrDefault();

                        if (signature != null)
                        {
                            var signatureBytes = Convert.FromBase64String(signature);
                            c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(signatureBytes, ImageScaling.FitHeight);
                        }

                        c.Item().AlignRight().Text(data.Value.headers["yourName"]?.ToString());
                        c.Item().AlignRight().Text(data.Value.headers["yourCity"] + ", " + data.Value.meeting.StartDate.ToLongDateString()).FontSize(10);
                    });
                });
            });
        });
    }

    private async Task<(Meeting meeting, Patient patient, Setting setting, JsonNode headers)?> GetData(string meetingId)
    {
        var meeting = await _mediator.Send(new FindOne<Meeting>(meetingId));

        if (meeting == null)
        {
            _logger.LogWarning("Cannot find meeting {meetingId}", meetingId);
            return null;
        }

        if (string.IsNullOrWhiteSpace(meeting.PatientId))
        {
            _logger.LogWarning("Cannot find patient for meeting {meetingId}", meetingId);
            return null;
        }

        var patient = await _mediator.Send(new FindOne<Patient>(meeting.PatientId));

        if (patient == null)
        {
            _logger.LogWarning("Cannot find patient {patientId}", meeting.PatientId);
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

        return (meeting, patient, setting, headers);
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

    private string ReplaceContent(string templateContent, Meeting meeting, Patient patient, JsonNode headers)
    {
        var template = Template.Parse(templateContent);

        var data = new
        {
            name = headers["yourName"]?.ToString(),
            patientName = patient.LastName + " " + patient.FirstName,
            price = meeting.Price.ToString("F2") + "€",
            meetingType = meeting.Type,
            meetingDate = meeting.StartDate.ToLongDateString(),
            meetingHour = meeting.StartDate.ToShortTimeString(),
            paymentDate = (meeting.PaymentDate ?? meeting.StartDate).ToString("dd/MM/yyyy"),
            paymentMode = _localizator.GetTranslation("documents", "payment" + meeting.Payment)
        };

        return template.Render(Hash.FromAnonymousObject(data));
    }
}