namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AppointmentUpdated : IDomainEvent
{
    required public Appointment Appointment { get; set; }
    public string StreamId => Appointment.Id;
}