namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

using Core.Attributes;

[StreamType(Streams.Appointments)]
public class AttachAppointmentDocument: BaseEvent
{
    required public BinaryDocument Document { get; set; }
}