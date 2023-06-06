namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastPrevisionCreated: BaseEvent
{
    public ForecastPrevision Prevision { get; set; } = new();
}