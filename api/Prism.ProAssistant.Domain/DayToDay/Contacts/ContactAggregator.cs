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

    public void When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(ContactCreated):
                Apply(@event.ToObject<ContactCreated>());
                break;
            case nameof(ContactUpdated):
                Apply(@event.ToObject<ContactUpdated>());
                break;
            default:
                throw new NotImplementedException($"The event type {@event.Type} is not implemented");
        }
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