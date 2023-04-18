namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AttachAppointmentDocument: IDomainEvent
{
    required public BinaryDocument Document { get; set; }
    required public string StreamId { get; set; }
}