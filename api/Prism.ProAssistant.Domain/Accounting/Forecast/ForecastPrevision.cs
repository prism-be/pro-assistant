namespace Prism.ProAssistant.Domain.Accounting.Forecast;

using System.Text.Json.Serialization;
using Core;

public enum ForecastPrevisionType
{
    Income = 0,
    Expense = 1
}

public enum RecurringType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3
}

public class ForecastPrevision
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = Identifier.GenerateString();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("recurringCount")]
    public int RecurringCount { get; set; }

    [JsonPropertyName("recurringType")]
    public RecurringType RecurringType { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("type")]
    public ForecastPrevisionType Type { get; set; }
}