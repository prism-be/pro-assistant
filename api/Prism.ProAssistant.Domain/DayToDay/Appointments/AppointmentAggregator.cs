namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using Configuration.Tariffs;
using Events;

public class AppointmentAggregator : IDomainAggregator<Appointment>
{
    private readonly IHydrator _hydrator;
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
            case nameof(AppointmentContactUpdated):
                Apply(@event.ToEvent<AppointmentContactUpdated>());
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }

        await EnsureReferences();
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

    private async Task EnsureReferences()
    {
        if (_state.Type != _tariff?.Id)
        {
            _tariff = await _hydrator.Hydrate<Tariff>(_state.Type);

            State.ForeColor = _tariff?.ForeColor;
            State.BackgroundColor = _tariff?.BackgroundColor;
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