namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

using Core.Attributes;

[StreamType(Streams.Appointments)]
public class DetachAppointmentDocument : BaseEvent
{
    required public string DocumentId { get; set; }
}