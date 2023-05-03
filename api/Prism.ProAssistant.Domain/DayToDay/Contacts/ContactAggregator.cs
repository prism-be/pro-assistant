using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

namespace Prism.ProAssistant.Domain.DayToDay.Contacts;

public class ContactAggregator : IDomainAggregator<Contact>
{
    private string? _id;
    private Contact? _state;

    public void Init(string id)
    {
        _id = id;
        _state = new Contact
        {
            Id = id
        };
    }

    public Contact State => _state ?? throw new InvalidOperationException("The state has not been initialized");

    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(ContactCreated):
                Apply(@event.ToEvent<ContactCreated>());
                break;
            case nameof(ContactUpdated):
                Apply(@event.ToEvent<ContactUpdated>());
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

    private void Apply(ContactCreated @event)
    {
        _state = @event.Contact;
        _state.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");
    }

    private void Apply(ContactUpdated @event)
    {
        _state = @event.Contact;
        _state.Id = _id ?? throw new InvalidOperationException("The id has not been initialized");
    }
}