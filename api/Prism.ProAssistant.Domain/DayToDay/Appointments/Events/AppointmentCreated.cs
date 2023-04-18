namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AppointmentCreated : IDomainEvent
{
    required public Appointment Appointment { get; set; }
    public string StreamId => Appointment.Id;
}