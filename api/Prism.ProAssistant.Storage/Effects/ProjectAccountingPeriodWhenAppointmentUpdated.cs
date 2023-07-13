namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Appointments.Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(AppointmentCreated))]
[SideEffect(typeof(AppointmentUpdated))]
public class ProjectAccountingPeriodWhenAppointmentUpdated
{
    private readonly ILogger<ProjectAccountingPeriodWhenAppointmentUpdated> _logger;
    private readonly IQueryService _queryService;
    private readonly IStateProvider _stateProvider;

    public ProjectAccountingPeriodWhenAppointmentUpdated(ILogger<ProjectAccountingPeriodWhenAppointmentUpdated> logger, IQueryService queryService, IStateProvider stateProvider)
    {
        _logger = logger;
        _queryService = queryService;
        _stateProvider = stateProvider;
    }

    public async Task Handle(DomainEvent @event)
    {
        var appointment = await _queryService.SingleAsync<Appointment>(@event.StreamId);
        _logger.LogInformation("Projecting accounting period from change on appointment {AppointmentId}", appointment.Id);

        var startPeriod = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, 1);
        var endPeriod = startPeriod.AddMonths(1).AddDays(-1);

        var appointments = await _queryService.SearchAsync<Appointment>(
            new Filter(nameof(Appointment.StartDate), startPeriod, FilterOperator.GreaterThanOrEqual),
            new Filter(nameof(Appointment.StartDate), endPeriod, FilterOperator.LessThanOrEqual)
        );

        var accountingPeriod = AccountingReportingPeriodProjection.Project(12, appointments);
        var container = await _stateProvider.GetContainerAsync<AccountingReportingPeriod>();
        await container.WriteAsync(accountingPeriod.Id, accountingPeriod);
    }
}