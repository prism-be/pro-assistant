namespace Prism.ProAssistant.Domain.Accounting.Reporting;

using System.Text.Json.Serialization;
using Core.Attributes;

[Collection(Streams.AccountingReportingPeriod)]
public class AccountingReportingPeriod
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }
    
    [JsonPropertyName("type")]
    public int Type { get; set; }
    
    [JsonPropertyName("income")]
    public decimal Income { get; set; }
}