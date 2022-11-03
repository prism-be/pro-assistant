// -----------------------------------------------------------------------
//  <copyright file = "ReceiptGenerator.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Prism.ProAssistant.Documents.Generators;

public interface IReceiptGenerator
{
    Task<byte[]?> Generate(string meetingId);
}

public class ReceiptGenerator : IReceiptGenerator
{
    private readonly IOrganizationContext _organizationContext;
    private readonly ILocalizator _localizator;

    public ReceiptGenerator(IOrganizationContext organizationContext, ILocalizator localizator)
    {
        _organizationContext = organizationContext;
        _localizator = localizator;
    }

    public async Task<byte[]?> Generate(string meetingId)
    {
        var meetings = await _organizationContext.Meetings.FindAsync(Builders<Meeting>.Filter.Eq("Id", meetingId));
        var meeting = await meetings.SingleOrDefaultAsync();

        if (meeting == null)
        {
            return null;
        }
        
        var settings = await _organizationContext.Settings.FindAsync(Builders<Setting>.Filter.Eq("Id", "documents-headers"));
        var setting = await settings.SingleOrDefaultAsync();

        if (setting == null)
        {
            return null;
        }
        
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_localizator.Locale);
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(14));

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(2, Unit.Centimetre);
                        columns.ConstantColumn(8, Unit.Centimetre);
                        columns.RelativeColumn();
                    });

                    table.Cell().Row(1).Column(1).Element(e => e.Height(2, Unit.Centimetre)).Image(Placeholders.Image);
                    table.Cell().Row(1).Column(2).ColumnSpan(2).PaddingLeft(0.5f, Unit.Centimetre).Column(c =>
                    {
                        c.Item().Text(Placeholders.Name()).FontSize(10);
                        c.Item().Text(Placeholders.Name()).FontSize(10);
                        c.Item().Text(Placeholders.Name()).FontSize(10);
                        c.Item().Text(Placeholders.Name()).FontSize(10);
                    });

                    table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingTop(0.5f, Unit.Centimetre).Element(e => e.Height(0.25f, Unit.Centimetre)).LineHorizontal(1);

                    table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(Placeholders.LongDate()).FontSize(10).LineHeight(0.75f);

                    table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(Placeholders.Name());
                        c.Item().Text(Placeholders.Name());
                        c.Item().Text(Placeholders.Name());
                        c.Item().Text(Placeholders.Name());
                    });

                    table.Cell().Row(5).Column(1).ColumnSpan(3).Element(e => e.Height(10, Unit.Centimetre)).Column(c =>
                    {
                        c.Item().Text(_localizator.GetTranslation("receipt", "title")).Bold();
                        c.Item().PaddingTop(0.5f, Unit.Centimetre).Text(Placeholders.LoremIpsum());
                    });

                    table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
                    {
                        c.Item().AlignRight().Text(Placeholders.Name());
                        c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(Placeholders.Image);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}