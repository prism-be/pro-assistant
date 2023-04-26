using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

namespace Prism.ProAssistant.Domain.Configuration.Tariffs;

public class TariffAggregator : IDomainAggregator<Tariff>
{

    public void Init(string id)
    {
        State = new Tariff { Id = id, Name = string.Empty };
    }

    public Tariff? State { get; private set; }

    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(TariffCreated):
                Apply(@event.ToEvent<TariffCreated>());
                break;
            case nameof(TariffUpdated):
                Apply(@event.ToEvent<TariffUpdated>());
                break;
        }
        
        return Task.CompletedTask;
    }

    private void Apply(TariffCreated @event)
    {
        State = @event.Tariff;
    }

    private void Apply(TariffUpdated @event)
    {
        State = @event.Tariff;
    }
}