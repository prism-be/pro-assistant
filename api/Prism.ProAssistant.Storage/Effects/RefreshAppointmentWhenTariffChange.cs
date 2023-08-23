namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain.Configuration.Tariffs;
using Domain.DayToDay.Appointments;
using Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(Tariff))]
public class RefreshAppointmentWhenTariffChange
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<RefreshAppointmentWhenTariffChange> _logger;
    private readonly IQueryService _queryService;

    public RefreshAppointmentWhenTariffChange(ILogger<RefreshAppointmentWhenTariffChange> logger, IQueryService queryService, IEventStore eventStore)
    {
        _logger = logger;
        _queryService = queryService;
        _eventStore = eventStore;
    }

    public async Task Handle(EventContext<Tariff> context)
    {
        if (context.CurrentState == null)
        {
            return;
        }
        
        if (context.CurrentState?.BackgroundColor == context.PreviousState?.BackgroundColor)
        {
            return;
        }

        _logger.LogInformation("Refreshing appointments for tariff {TariffId}", context.Event.StreamId);
        var appointments = await _queryService.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.TypeId), context.CurrentState.Id));

        foreach (var appointment in appointments)
        {
            await _eventStore.Persist<Appointment>(appointment);
        }
    }
}