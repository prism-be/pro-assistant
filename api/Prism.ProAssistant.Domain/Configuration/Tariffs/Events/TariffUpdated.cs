namespace Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

public class TariffUpdated : IDomainEvent
{
    required public Tariff Tariff { get; set; }
    public string StreamId => Tariff.Id;
    public string StreamType => "tariffs";
}