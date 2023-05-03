namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using Configuration.Tariffs;
using Contacts;
using Events;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private readonly IHydrator _hydrator;

    private string? _id;

    public AppointmentAggregator(IHydrator hydrator)
    {
        _hydrator = hydrator;
    }


    public void Init(string id)
    {
        _id = id;
        State = new Appointment
        {
            Id = id,
            FirstName = string.Empty,
            LastName = string.Empty,
            Title = string.Empty
        };
    }

    public Appointment? State { get; private set; }

    public Task When(DomainEvent @event)
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
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }

        return Task.CompletedTask;
    }

    public async Task Complete()
    {
        if (State == null)
        {
            return;
        }

        var tariff = await _hydrator.Hydrate<Tariff>(State.TypeId);

        State.ForeColor = tariff?.ForeColor;
        State.BackgroundColor = tariff?.BackgroundColor;

        var contact = await _hydrator.Hydrate<Contact>(State.ContactId);

        State.FirstName = contact?.FirstName ?? string.Empty;
        State.LastName = contact?.LastName ?? string.Empty;
        State.BirthDate = contact?.BirthDate;
        State.PhoneNumber = contact?.PhoneNumber;
        State.Title = $"{contact?.LastName} {contact?.FirstName}";
    }

    private void Apply(DetachAppointmentDocument @event)
    {
        State = EnsureState();
        State.Documents.Remove(State.Documents.Single(x => x.Id == @event.DocumentId));
    }

    private void Apply(AttachAppointmentDocument @event)
    {
        State = EnsureState();
        State.Documents.Insert(0, @event.Document);
    }

    private void Apply(AppointmentCreated @event)
    {
        State = @event.Appointment.ToAppointment();
        State.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");
    }

    private void Apply(AppointmentUpdated @event)
    {
        State = EnsureState();

        @event.Appointment.ToAppointment(State);
        State.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");

        if (State.State == (int)AppointmentState.Canceled)
        {
            State = null;
        }
    }

    private Appointment EnsureState()
    {
        if (State == null)
        {
            throw new InvalidOperationException("The state has not been initialized");
        }

        return State;
    }
}