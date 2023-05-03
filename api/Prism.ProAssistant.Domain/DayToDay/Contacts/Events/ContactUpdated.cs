namespace Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

using Core.Attributes;

[StreamType(Streams.Contacts)]
public class ContactUpdated : BaseEvent
{
    required public Contact Contact { get; set; }
    public override string StreamId => Contact.Id;
}