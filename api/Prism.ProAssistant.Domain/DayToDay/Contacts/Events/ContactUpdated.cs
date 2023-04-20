namespace Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

public class ContactUpdated : IDomainEvent
{
    required public Contact Contact { get; set; }
    public string StreamId => Contact.Id;
    public string StreamType => "contacts";
}