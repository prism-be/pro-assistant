namespace Prism.ProAssistant.Domain.Accounting.Forecast;

using System.Text.Json.Serialization;

public class ForecastWeeklyBudget
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("monday")]
    public DateTime Monday { get; set; }

    [JsonPropertyName("weekOfYear")]
    public int WeekOfYear { get; set; }
}