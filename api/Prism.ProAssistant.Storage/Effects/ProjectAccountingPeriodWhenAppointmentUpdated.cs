namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain.DayToDay.Appointments;
using Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(Appointment))]
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

    public async Task Handle(EventContext<Appointment> context)
    {
        _logger.LogInformation("Projecting accounting period from change on appointment {AppointmentId}", context.Event.StreamId);

        if (context.CurrentState != null)
        {
            await Project(context.CurrentState);
        }

        if (context.PreviousState != null)
        {
            await Project(context.PreviousState);
        }
    }

    private async Task Project(Appointment appointment)
    {
        var startPeriod = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endPeriod = startPeriod.AddMonths(1);

        await ProjectAccountingPeriodBase.Project(startPeriod, endPeriod, _queryService, _stateProvider);
    }
}