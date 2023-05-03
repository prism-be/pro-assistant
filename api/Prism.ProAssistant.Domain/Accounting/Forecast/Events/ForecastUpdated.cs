namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastUpdated: BaseEvent
{
    public string Title { get; set; } = string.Empty;
}