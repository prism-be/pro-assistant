namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using Configuration.Tariffs;
using Contacts;
using Events;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private readonly IHydrator _hydrator;

    private Contact? _contact;
    private string? _id;

    private Tariff? _tariff;

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

    public async Task When(DomainEvent @event)
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

        await EnsureReferences();
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

    private async Task EnsureReferences()
    {
        if (State == null)
        {
            return;
        }

        if (State.TypeId != _tariff?.Id)
        {
            _tariff = await _hydrator.Hydrate<Tariff>(State.TypeId);

            State.ForeColor = _tariff?.ForeColor;
            State.BackgroundColor = _tariff?.BackgroundColor;
        }

        if (State.ContactId != _contact?.Id)
        {
            _contact = await _hydrator.Hydrate<Contact>(State.ContactId);

            State.FirstName = _contact?.FirstName ?? string.Empty;
            State.LastName = _contact?.LastName ?? string.Empty;
            State.BirthDate = _contact?.BirthDate;
            State.PhoneNumber = _contact?.PhoneNumber;
            State.Title = $"{_contact?.LastName} {_contact?.FirstName}";
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