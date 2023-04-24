namespace Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

public class AppointmentContactUpdated: IDomainEvent
{
    required public string FirstName { get; set; }
    required public string LastName { get; set; }
    required public string Title { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BirthDate { get; set; }
    
    required public string StreamId { get; set; }
    public string StreamType => "appointments";
}