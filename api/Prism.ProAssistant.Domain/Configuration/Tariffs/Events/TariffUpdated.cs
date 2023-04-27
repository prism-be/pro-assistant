namespace Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

using Core.Attributes;

[StreamType("tariffs")]
public class TariffUpdated : IDomainEvent
{
    required public Tariff Tariff { get; set; }
    public string StreamId => Tariff.Id;
    public string StreamType => StreamTypeAttribute.GetStreamType(this.GetType());
}