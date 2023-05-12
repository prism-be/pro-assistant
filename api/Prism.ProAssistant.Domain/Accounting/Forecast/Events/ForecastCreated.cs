namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastCreated: BaseEvent
{
    public string Title { get; set; } = string.Empty;

    public int Year { get; set; } = DateTime.Today.Year;
}