namespace Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

public class ContactCreated : IDomainEvent
{
    required public Contact Contact { get; set; }
    public string StreamId => Contact.Id;
}