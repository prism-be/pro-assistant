namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastPrevisionDeleted : BaseEvent
{
    public string Id { get; set; } = string.Empty;
}