using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private string? _id;
    private Appointment? _state;

    public void Init(string id)
    {
        _id = id;
        _state = new Appointment
        {
            Id = id,
            FirstName = string.Empty,
            LastName = string.Empty,
            Title = string.Empty
        };
    }

    public Appointment State => _state ?? throw new InvalidOperationException("The state has not been initialized");

    public void When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(AppointmentCreated):
                Apply(@event.ToEvent<AppointmentCreated>());
                break;
            case nameof(AppointmentUpdated):
                Apply(@event.ToEvent<AppointmentUpdated>());
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }
    }

    private void Apply(AppointmentCreated @event)
    {
        _state = @event.Appointment;
        _state.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");
    }

    private void Apply(AppointmentUpdated @event)
    {
        _state = @event.Appointment;
        _state.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");
    }
}