namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using Configuration.Tariffs;
using Contacts;
using Events;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private readonly IHydrator _hydrator;

    private Contact? _contact;
    private string? _id;
    private Appointment _state = null!;

    private Tariff? _tariff;

    public AppointmentAggregator(IHydrator hydrator)
    {
        _hydrator = hydrator;
    }


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

    private async Task EnsureReferences()
    {
        if (_state.TypeId != _tariff?.Id)
        {
            _tariff = await _hydrator.Hydrate<Tariff>(_state.TypeId);

            State.ForeColor = _tariff?.ForeColor;
            State.BackgroundColor = _tariff?.BackgroundColor;
        }

        if (_state.ContactId != _contact?.Id)
        {
            _contact = await _hydrator.Hydrate<Contact>(_state.ContactId);

            State.FirstName = _contact?.FirstName ?? string.Empty;
            State.LastName = _contact?.LastName ?? string.Empty;
            State.BirthDate = _contact?.BirthDate;
            State.PhoneNumber = _contact?.PhoneNumber;
            State.Title = $"{_contact?.LastName} {_contact?.FirstName}";
        }
    }

    private void EnsureState()
    {
        if (_state == null)
        {
            throw new InvalidOperationException("The state has not been initialized");
        }
    }
}