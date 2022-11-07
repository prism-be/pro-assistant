// -----------------------------------------------------------------------
//  <copyright file = "ReceiptGenerator.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using DotLiquid;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace Prism.ProAssistant.Documents.Generators;

public interface IReceiptGenerator
{
    Task<byte[]?> Generate(string meetingId);
}

public class ReceiptGenerator : IReceiptGenerator
{
    private readonly IOrganizationContext _organizationContext;
    private readonly ILocalizator _localizator;
    private readonly ILogger<ReceiptGenerator> _logger;

    public ReceiptGenerator(IOrganizationContext organizationContext, ILocalizator localizator, ILogger<ReceiptGenerator> logger)
    {
        _organizationContext = organizationContext;
        _localizator = localizator;
        _logger = logger;
    }

    public async Task<byte[]?> Generate(string meetingId)
    {
        var meetings = await _organizationContext.Meetings.FindAsync(Builders<Meeting>.Filter.Eq("Id", meetingId));
        var meeting = await meetings.SingleOrDefaultAsync();

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
        
        var patients = await _organizationContext.Patients.FindAsync(Builders<Patient>.Filter.Eq("Id", meeting.PatientId));
        var patient = await patients.SingleOrDefaultAsync();

        if (patient == null)
        {
            _logger.LogWarning("Cannot find patient {patientId}", meeting.PatientId);
            return null;
        }

        var settings = await _organizationContext.Settings.FindAsync(Builders<Setting>.Filter.Eq("Id", "documents-headers"));
        var setting = await settings.SingleOrDefaultAsync();

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

        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);

        var document = Document.Create(container =>
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

                    var logo = headers["logo"]?.ToString().Split(',').LastOrDefault();

                    if (logo != null)
                    {
                        var logoBytes = Convert.FromBase64String(logo);
                        table.Cell().Row(1).Column(1).Element(e => e.Height(2, Unit.Centimetre)).Image(logoBytes);
                    }

                    table.Cell().Row(1).Column(2).ColumnSpan(2).PaddingLeft(0.5f, Unit.Centimetre).Column(c =>
                    {
                        c.Item().Text(headers["name"]).FontSize(10);

                        foreach (var line in headers["address"]?.ToString().Split('\n') ?? Array.Empty<string>())
                        {
                            c.Item().Text(line).FontSize(10);
                        }
                    });

                    table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingTop(0.5f, Unit.Centimetre).Element(e => e.Height(0.25f, Unit.Centimetre)).LineHorizontal(1);

                    table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(meeting.StartDate.ToLongDateString()).FontSize(10).LineHeight(0.75f);

                    table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(patient.LastName + " " + patient.FirstName);
                        c.Item().Text(patient.Street + " " + patient.Number);
                        c.Item().Text(patient.ZipCode + " " + patient.City);
                        c.Item().Text(patient.Country);
                    });

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(10, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(_localizator.GetTranslation("receipt", "title") + " " + meeting.Type).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(GetContent(meeting, patient, headers));
                    });

                    table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
                    {

                        var signature = headers["signature"]?.ToString().Split(',').LastOrDefault();

                        if (signature != null)
                        {
                            var signatureBytes = Convert.FromBase64String(signature);
                            c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(signatureBytes, ImageScaling.FitHeight);
                        }
                        
                        c.Item().AlignRight().Text(headers["yourName"]);
                        c.Item().AlignRight().Text(headers["yourCity"] + ", " + meeting.StartDate.ToLongDateString()).FontSize(10);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private string GetContent(Meeting meeting, Patient patient, JsonNode headers)
    {
        var template = Template.Parse(_localizator.GetTranslation("receipt", "content"));

        var data = new
        {
            name = headers["yourName"]?.ToString(),
            patientName = patient.LastName + " " + patient.FirstName,
            price = meeting.Price.ToString("F2") + "€",
            meetingType = meeting.Type,
            meetingDate = meeting.StartDate.ToLongDateString(),
            meetingHour = meeting.StartDate.ToShortTimeString(),
            paymentDate = (meeting.PaymentDate ?? meeting.StartDate).ToString("dd/MM/yyyy"),
            paymentMode = _localizator.GetTranslation("receipt", "payment" + meeting.Payment)
        };

        return template.Render(Hash.FromAnonymousObject(data));
    }
}