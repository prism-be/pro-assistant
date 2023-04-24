namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AppointmentColorUpdated: IDomainEvent
{
    public string? ForeColor { get; set; }
    public string? BackgroundColor { get; set; }
    
    required public string StreamId { get; set; }
    public string StreamType => "appointments";
}