using System.Globalization;
using Acme.Core.Extensions;
using DotLiquid;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Helpers;
using Prism.ProAssistant.Api.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace Prism.ProAssistant.Api.Services;

public interface IPdfService
{
    Task GenerateDocument([FromBody] DocumentRequest request);
}

public class PdfService : IPdfService
{
    private readonly IDataService _dataService;
    private readonly ILogger<PdfService> _logger;

    public PdfService(IDataService dataService, ILogger<PdfService> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task GenerateDocument([FromBody] DocumentRequest request)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-be");
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-be");

        var documentConfiguration = await _dataService.SingleAsync<DocumentConfiguration>(request.DocumentId);
        var appointment = await _dataService.SingleAsync<Appointment>(request.AppointmentId);

        var contact = !string.IsNullOrEmpty(appointment.ContactId) ? await _dataService.SingleAsync<Contact>(appointment.ContactId) : null;

        var (title, content) = ReplaceContent(documentConfiguration, appointment, contact);

        var document = CreateDocument(appointment, contact, title, content);
        var bytes = document.GeneratePdf();
        await SaveDocument(appointment, title, bytes);
    }

    private static void WriteContactAddress(TableDescriptor table, Appointment appointment, Contact? contact)
    {
        table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(DateTime.Today.ToLongDateString()).FontSize(10).LineHeight(0.75f);

        table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
        {
            c.Item().Text($"{contact?.Title} {contact?.LastName ?? appointment.LastName} {contact?.FirstName ?? appointment.FirstName}".Trim());
            c.Item().Text($"{contact?.Street} {contact?.Number}".Trim());
            c.Item().Text($"{contact?.ZipCode} {contact?.City}".Trim());
            c.Item().Text(contact?.Country);
        });
    }

    private Document CreateDocument(Appointment appointment, Contact? contact, string title, string content)
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
                    WriteHeader(table);
                    WriteContactAddress(table, appointment, contact);

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(13, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(ReplaceContent(title, appointment, contact)).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(ReplaceContent(content, appointment, contact));
                    });

                    WriteSignature(table);
                });
            });
        });
    }

    private static string GetPaymentTranslation(int payment)
    {
        switch (payment)
        {
            case 0:
                return "Non payé";
            case 1:
                return "Payé en espèces";
            case 2:
                return "Payé par virement";
            case 3:
                return "Payé par carte bancaire";
        }

        throw new NotImplementedException($"Payment mode {payment} not implemented.");
    }

    private string GetSettingValue(string id)
    {
        var task = _dataService.SingleAsync<Setting>(id);
        task.Wait();
        return task.Result?.Value ?? throw new MissingConfigurationException("Setting not found.", id);
    }

    private (string title, string content) ReplaceContent(DocumentConfiguration documentConfiguration, Appointment appointment, Contact? contact)
    {
        return (
            ReplaceContent(documentConfiguration.Title ?? string.Empty, appointment, contact),
            ReplaceContent(documentConfiguration.Body ?? string.Empty, appointment, contact)
        );
    }

    private string ReplaceContent(string templateContent, Appointment appointment, Contact? contact)
    {
        var template = Template.Parse(templateContent);

        var data = new
        {
            name = GetSettingValue("document-header-your-name"),
            contactName = (contact?.Title + " " + (contact?.LastName ?? appointment.LastName) + " " + (contact?.FirstName ?? appointment.FirstName)).Trim(),
            price = appointment.Price.ToString("F2") + "€",
            appointmentType = appointment.Type,
            appointmentDate = appointment.StartDate.ToLongDateString(),
            appointmentHour = appointment.StartDate.ToShortTimeString(),
            paymentDate = (appointment.PaymentDate ?? appointment.StartDate).ToString("dd/MM/yyyy"),
            paymentMode = GetPaymentTranslation(appointment.Payment)
        };

        return template.Render(Hash.FromAnonymousObject(data));
    }

    private async Task SaveDocument(Appointment appointment, string title, byte[] bytes)
    {
        var documentTitle = $"{appointment.StartDate:yyyy-MM-dd HH:mm} - {appointment.LastName} {appointment.FirstName} - {title}";
        var fileName = documentTitle.ReplaceSpecialChars(true) + ".pdf";

        var fileId = await _dataService.UploadFromBytesAsync(fileName, bytes);

        var document = new BinaryDocument
        {
            Id = fileId,
            Title = documentTitle,
            Date = DateTime.UtcNow,
            FileName = fileName
        };

        appointment.Documents.Insert(0, document);

        await _dataService.ReplaceAsync(appointment);

        _logger.LogInformation("Document {itemId} was saved for Appointment {appointmentId}", document.Id, appointment.Id);
    }

    private void WriteHeader(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(2, Unit.Centimetre);
            columns.ConstantColumn(8, Unit.Centimetre);
            columns.RelativeColumn();
        });

        var logo = GetSettingValue("document-header-logo").Split(',').LastOrDefault();

        if (logo != null)
        {
            var logoBytes = Convert.FromBase64String(logo);
            logoBytes = ImageProcessor.Resize(logoBytes, 250, 250);
            table.Cell().Row(1).Column(1).Element(e => e.Height(2, Unit.Centimetre)).Image(logoBytes);
        }

        table.Cell().Row(1).Column(2).ColumnSpan(2).PaddingLeft(0.5f, Unit.Centimetre).Column(c =>
        {
            c.Item().Text(GetSettingValue("document-header-name")).FontSize(10);

            foreach (var line in GetSettingValue("document-header-address").Split('\n'))
            {
                c.Item().Text(line).FontSize(10);
            }
        });

        table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingTop(0.5f, Unit.Centimetre)
            .Element(e => e.Height(0.25f, Unit.Centimetre))
            .LineHorizontal(0.5f)
            .LineColor(GetSettingValue("document-header-accentuate-color"));
    }

    private void WriteSignature(TableDescriptor table)
    {
        table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
        {
            var signature = GetSettingValue("document-header-signature").Split(',').LastOrDefault();

            if (signature != null)
            {
                var signatureBytes = Convert.FromBase64String(signature);
                signatureBytes = ImageProcessor.Resize(signatureBytes, 400, 200);
                c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(signatureBytes, ImageScaling.FitHeight);
            }

            c.Item().AlignRight().Text(GetSettingValue("document-header-your-name"));
            c.Item().AlignRight().Text(GetSettingValue("document-header-your-city") + ", " + DateTime.Today.ToLongDateString()).FontSize(10);
        });
    }
}