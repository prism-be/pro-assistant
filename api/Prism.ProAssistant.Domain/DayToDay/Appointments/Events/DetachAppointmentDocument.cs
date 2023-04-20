namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class DetachAppointmentDocument : IDomainEvent
{
    required public string DocumentId { get; set; }
    required public string StreamId { get; set; }
    public string StreamType => "appointments";
}