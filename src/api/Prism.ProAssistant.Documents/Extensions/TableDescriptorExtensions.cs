// -----------------------------------------------------------------------
//  <copyright file = "TableDescriptorExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Prism.ProAssistant.Business.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Prism.ProAssistant.Documents.Extensions;

public static class TableDescriptorExtensions
{
    public static void WriteContactAddress(this TableDescriptor table, Contact contact)
    {
        table.Cell().Row(3).Column(1).ColumnSpan(3).AlignRight().Text(DateTime.Today.ToLongDateString()).FontSize(10).LineHeight(0.75f);

        table.Cell().Row(4).Column(3).PaddingTop(1, Unit.Centimetre).Element(e => e.Height(5, Unit.Centimetre)).Column(c =>
        {
            c.Item().Text(contact.LastName + " " + contact.FirstName);
            c.Item().Text(contact.Street + " " + contact.Number);
            c.Item().Text(contact.ZipCode + " " + contact.City);
            c.Item().Text(contact.Country);
        });
    }
    
    public static void WriteHeader(this TableDescriptor table, JsonNode headers)
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
            logoBytes = Helpers.ResizeImage(logoBytes, 250, 250);
            table.Cell().Row(1).Column(1).Element(e => e.Height(2, Unit.Centimetre)).Image(logoBytes);
        }

        table.Cell().Row(1).Column(2).ColumnSpan(2).PaddingLeft(0.5f, Unit.Centimetre).Column(c =>
        {
            c.Item().Text(headers["name"]?.ToString()).FontSize(10);

            foreach (var line in headers["address"]?.ToString().Split('\n') ?? Array.Empty<string>())
            {
                c.Item().Text(line).FontSize(10);
            }
        });

        table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingTop(0.5f, Unit.Centimetre).Element(e => e.Height(0.25f, Unit.Centimetre)).LineHorizontal(0.5f);
    }

    public static void WriteSignature(this TableDescriptor table, JsonNode headers)
    {
        table.Cell().Row(6).Column(3).PaddingTop(1, Unit.Centimetre).Column(c =>
        {
            var signature = headers["signature"]?.ToString().Split(',').LastOrDefault();

            if (signature != null)
            {
                var signatureBytes = Convert.FromBase64String(signature);
                signatureBytes = Helpers.ResizeImage(signatureBytes, 400, 200);
                c.Item().AlignRight().Element(e => e.Height(2, Unit.Centimetre)).Image(signatureBytes, ImageScaling.FitHeight);
            }

            c.Item().AlignRight().Text(headers["yourName"]?.ToString());
            c.Item().AlignRight().Text(headers["yourCity"] + ", " + DateTime.Today.ToLongDateString()).FontSize(10);
        });
    }
}