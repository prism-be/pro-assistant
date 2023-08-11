namespace Prism.ProAssistant.Domain.Accounting.Reporting;

using DayToDay.Appointments;

public static class AccountingReportingPeriodProjection
{
    public static AccountingReportingPeriod Project(int periodType, IEnumerable<Appointment> appointments)
    {
        var appointmentsEnumerated = appointments.ToList();
        var firstAppointmentDate = appointmentsEnumerated.MinBy(x => x.StartDate)?.StartDate ?? DateTime.Now;
        var startDate = new DateTime(firstAppointmentDate.Year, firstAppointmentDate.Month, 1);

        var endDate = periodType switch
        {
            1 => startDate.AddYears(1).AddDays(-1),
            12 => startDate.AddMonths(1).AddDays(-1),
            52 => startDate.AddDays(6),
            _ => throw new NotSupportedException("Period type not supported. Please use 1, 12 or 52.")
        };

        var period = new AccountingReportingPeriod
        {
            Id = $"{startDate:yyyy-MM-dd}-{periodType:00}",
            StartDate = startDate,
            EndDate = endDate,
            Type = periodType,
            Income = 0
        };

        foreach (var appointment in appointmentsEnumerated)
        {
            if (appointment.Payment == (int)PaymentTypes.Unpayed || appointment.State == (int)AppointmentState.Canceled)
            {
                continue;
            }
            
            period.Income += appointment.Price;
        }

        return period;
    }
}