﻿namespace Prism.ProAssistant.Domain.Accounting.Document;

using System.Text.Json.Serialization;
using Core.Attributes;

[Collection(Streams.AccountingDocument)]
public class AccountingDocument
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
}