namespace Prism.ProAssistant.Domain.DayToDay.Appointments;

using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AppointmentInformationMapper
{
    public static partial Appointment ToAppointment(this AppointmentInformation appointmentInformation);
    public static partial void ToAppointment(this AppointmentInformation appointmentInformation, Appointment appointment);
}