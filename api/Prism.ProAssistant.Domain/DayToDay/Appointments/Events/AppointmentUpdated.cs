namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AppointmentUpdated : IDomainEvent
{
    required public AppointmentInformation Appointment { get; set; }
    public string StreamId => Appointment.Id;
    public string StreamType => "appointments";
}