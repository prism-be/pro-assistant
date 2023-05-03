namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

using Core.Attributes;

[StreamType(Streams.Appointments)]
public class AppointmentCreated : BaseEvent
{
    required public AppointmentInformation Appointment { get; set; }
    public override string StreamId => Appointment.Id;
}