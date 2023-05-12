namespace Prism.ProAssistant.Domain.Accounting.Forecast;

using System.Text.Json.Serialization;
using Core.Attributes;

[Collection(Streams.AccountingForecast)]
public class Forecast
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("id")]
    required public string Id { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("previsions")]
    public List<ForecastPrevision> Previsions { get; set; } = new ();

    [JsonPropertyName("weeklyBudgets")]
    public List<ForecastWeeklyBudget> WeeklyBudgets { get; set; } = new();


}