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
            case nameof(ForecastPrevisionCreated):
                Apply(@event.ToEvent<ForecastPrevisionCreated>());
                break;
            case nameof(ForecastPrevisionUpdated):
                Apply(@event.ToEvent<ForecastPrevisionUpdated>());
                break;
            case nameof(ForecastPrevisionDeleted):
                Apply(@event.ToEvent<ForecastPrevisionDeleted>());
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }

        return Task.CompletedTask;
    }

    public Task Complete()
    {
        var calculator = new ForecastWeeklyBudgetCalculator(EnsureState());
        calculator.Compute();
        
        return Task.CompletedTask;
    }

    private void Apply(ForecastPrevisionCreated e)
    {
        State = EnsureState();
        State.Previsions.Add(e.Prevision);
    }

    private void Apply(ForecastPrevisionUpdated e)
    {
        State = EnsureState();
        var prevision = State.Previsions.Find(x => x.Id == e.Prevision.Id);
        if (prevision == null)
        {
            throw new InvalidOperationException($"The prevision {e.Prevision.Id} does not exist");
        }

        State.Previsions.Remove(prevision);
        State.Previsions.Add(e.Prevision);
    }

    private void Apply(ForecastPrevisionDeleted e)
    {
        State = EnsureState();
        var prevision = State.Previsions.Find(x => x.Id == e.Id);
        if (prevision == null)
        {
            throw new InvalidOperationException($"The prevision {e.Id} does not exist");
        }

        State.Previsions.Remove(prevision);
    }

    private void Apply(ForecastCreated e)
    {
        State = new Forecast
        {
            Id = e.StreamId,
            Title = e.Title,
            Year = e.Year
        };
    }

    private void Apply(ForecastUpdated e)
    {
        State = EnsureState();
        State.Title = e.Title;
        State.Year = e.Year;
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