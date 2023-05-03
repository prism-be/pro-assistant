namespace Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

using Core.Attributes;

[StreamType(Streams.Tariffs)]
public class TariffCreated: BaseEvent
{
    required public Tariff Tariff { get; set; }
    public override string StreamId => Tariff.Id;
}