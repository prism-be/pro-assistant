namespace Prism.ProAssistant.Domain.Accounting.Forecast;

using Core;
using Events;

public class ForecastAggregator : IDomainAggregator<Forecast>
{
    public void Init(string id)
    {
        State = new Forecast
        {
            Id = id
        };
    }

    public Forecast? State { get; private set; } = new()
    {
        Id = Identifier.GenerateString()
    };

    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(ForecastCreated):
                Apply(@event.ToEvent<ForecastCreated>());
                break;
            case nameof(ForecastUpdated):
                Apply(@event.ToEvent<ForecastUpdated>());
                break;
            case nameof(ForecastDeleted):
                Apply(@event.ToEvent<ForecastDeleted>());
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }

        return Task.CompletedTask;
    }

    public Task Complete()
    {
        return Task.CompletedTask;
    }

    private void Apply(ForecastCreated e)
    {
        State = new Forecast
        {
            Id = e.StreamId,
            Title = e.Title
        };
    }

    private void Apply(ForecastUpdated e)
    {
        State = EnsureState();
        State.Title = e.Title;
    }

    private void Apply(ForecastDeleted _)
    {
        State = null;
    }

    private Forecast EnsureState()
    {
        if (State == null)
        {
            throw new InvalidOperationException("The state has not been initialized");
        }

        return State;
    }
}