namespace Prism.ProAssistant.Domain.Accounting.Reporting;

using DayToDay.Appointments;
using Document;

public static class AccountingReportingPeriodProjection
{
    private static void AddAppointments(AccountingReportingPeriod period, List<Appointment> appointments)
    {
        foreach (var appointment in appointments)
        {
            if (appointment.Payment == (int)PaymentTypes.Unpayed || appointment.State == (int)AppointmentState.Canceled)
            {
                continue;
            }

            var detail = period.Details.Find(x => x.Type == "appointment" && x.UnitPrice == appointment.Price);
            if (detail == null)
            {
                detail = new IncomeDetail
                {
                    Type = "appointment",
                    UnitPrice = appointment.Price,
                    Count = 0,
                    SubTotal = 0
                };
                period.Details.Add(detail);
            }

            detail.Count++;
            detail.SubTotal += appointment.Price;

            period.Income += appointment.Price;
        }
    }

    private static void AddDocuments(AccountingReportingPeriod period, IEnumerable<AccountingDocument> documents)
    {
        foreach (var document in documents)
        {
            if (document.Amount > 0)
            {
                period.Income += document.Amount;
                var detail = period.Details.Find(x => x.Type == "document-income" && x.Category == document.Category);
                if (detail == null)
                {
                    detail = new IncomeDetail
                    {
                        Type = "document-income",
                        Category = document.Category,
                        UnitPrice = 0,
                        Count = 0,
                        SubTotal = 0
                    };
                    period.Details.Add(detail);
                }

                detail.Count = 1;
                detail.SubTotal += document.Amount;
                detail.UnitPrice = detail.SubTotal;
            }
            else
            {
                period.Expense += Math.Abs(document.Amount);
                var detail = period.Details.Find(x => x.Type == "document-expense" && x.Category == document.Category);
                if (detail == null)
                {
                    detail = new IncomeDetail
                    {
                        Type = "document-expense",
                        Category = document.Category,
                        UnitPrice = 0,
                        Count = 0,
                        SubTotal = 0
                    };
                    period.Details.Add(detail);
                }

                detail.Count = 1;
                detail.SubTotal -= Math.Abs(document.Amount);
                detail.UnitPrice = detail.SubTotal;
            }
        }
    }

    public static AccountingReportingPeriod Project(int periodType, IEnumerable<Appointment> appointments, IEnumerable<AccountingDocument> documents)
    {
        var appointmentsEnumerated = appointments.ToList();
        var firstAppointmentDate = appointmentsEnumerated.MinBy(x => x.StartDate)?.StartDate ?? DateTime.Now;
        var startDate = new DateTime(firstAppointmentDate.Year, firstAppointmentDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);

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
            Income = 0,
            Details = new List<IncomeDetail>()
        };

        AddAppointments(period, appointmentsEnumerated);
        AddDocuments(period, documents);

        return period;
    }
}