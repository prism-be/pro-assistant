namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastDeleted: BaseEvent
{
}