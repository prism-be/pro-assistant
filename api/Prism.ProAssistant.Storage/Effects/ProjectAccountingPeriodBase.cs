namespace Prism.ProAssistant.Storage.Effects;

using Domain.Accounting.Document;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using Infrastructure.Providers;

public static class ProjectAccountingPeriodBase
{
    public static async Task Project(DateTime startPeriod, DateTime endPeriod, IQueryService queryService, IStateProvider stateProvider)
    {
        var appointments = await queryService.SearchAsync<Appointment>(
            new Filter(nameof(Appointment.StartDate), startPeriod, FilterOperator.GreaterThanOrEqual),
            new Filter(nameof(Appointment.StartDate), endPeriod, FilterOperator.LessThan)
        );

        var documents = await queryService.SearchAsync<AccountingDocument>(
            new Filter(nameof(AccountingDocument.Date), startPeriod, FilterOperator.GreaterThanOrEqual),
            new Filter(nameof(AccountingDocument.Date), endPeriod, FilterOperator.LessThan)
        );

        var accountingPeriod = AccountingReportingPeriodProjection.Project(12, appointments, documents);
        var container = await stateProvider.GetContainerAsync<AccountingReportingPeriod>();
        await container.WriteAsync(accountingPeriod.Id, accountingPeriod);
    }
}