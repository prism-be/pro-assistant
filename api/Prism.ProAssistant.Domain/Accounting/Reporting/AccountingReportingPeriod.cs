namespace Prism.ProAssistant.Domain.Accounting.Reporting;

using System.Text.Json.Serialization;
using Core;
using Core.Attributes;

[Collection(Streams.AccountingReportingPeriod)]
public class AccountingReportingPeriod
{
    [JsonPropertyName("details")]
    public List<IncomeDetail> Details { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("income")]
    public decimal Income { get; set; }
    
    [JsonPropertyName("expense")]
    public decimal Expense { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }
}

public class IncomeDetail
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Identifier.GenerateString();
    
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("subTotal")]
    public decimal SubTotal { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }
}