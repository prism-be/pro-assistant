using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private string? _id;
    private Appointment _state = null!;

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
            case nameof(AttachAppointmentDocument):
                Apply(@event.ToEvent<AttachAppointmentDocument>());
                break;
            case nameof(DetachAppointmentDocument):
                Apply(@event.ToEvent<DetachAppointmentDocument>());
                break;
            case nameof(AppointmentContactUpdated):
                Apply(@event.ToEvent<AppointmentContactUpdated>());
                break;
            case nameof(AppointmentColorUpdated):
                Apply(@event.ToEvent<AppointmentColorUpdated>());
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }
    }

    private void Apply(AppointmentColorUpdated @event)
    {
        EnsureState();

        _state.ForeColor = @event.ForeColor;
        _state.BackgroundColor = @event.BackgroundColor;
    }

    private void EnsureState()
    {
        if (_state == null) throw new InvalidOperationException("The state has not been initialized");
    }

    private void Apply(AppointmentContactUpdated @event)
    {
        EnsureState();

        _state.FirstName = @event.FirstName;
        _state.LastName = @event.LastName;
        _state.Title = @event.Title;
        _state.PhoneNumber = @event.PhoneNumber;
        _state.BirthDate = @event.BirthDate;
    }

    private void Apply(DetachAppointmentDocument @event)
    {
        EnsureState();

        _state.Documents.Remove(_state.Documents.Single(x => x.Id == @event.DocumentId));
    }

    private void Apply(AttachAppointmentDocument @event)
    {
        EnsureState();

        _state.Documents.Insert(0, @event.Document);
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