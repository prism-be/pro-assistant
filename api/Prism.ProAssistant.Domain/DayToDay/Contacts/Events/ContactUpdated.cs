namespace Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

using Core.Attributes;

[StreamType("contacts")]
public class ContactUpdated : IDomainEvent
{
    required public Contact Contact { get; set; }
    public string StreamId => Contact.Id;
    public string StreamType => StreamTypeAttribute.GetStreamType<ContactUpdated>();
}