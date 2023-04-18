namespace Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

public class TariffCreated: IDomainEvent
{
    required public Tariff Tariff { get; set; }
    public string StreamId => Tariff.Id;
}