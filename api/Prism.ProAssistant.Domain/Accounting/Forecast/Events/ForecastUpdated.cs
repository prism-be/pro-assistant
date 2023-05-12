namespace Prism.ProAssistant.Domain.Accounting.Forecast.Events;

using Core.Attributes;

[StreamType(Streams.AccountingForecast)]
public class ForecastUpdated: BaseEvent
{
    public int Year { get; set; } = DateTime.Today.Year;
    
    public string Title { get; set; } = string.Empty;
}