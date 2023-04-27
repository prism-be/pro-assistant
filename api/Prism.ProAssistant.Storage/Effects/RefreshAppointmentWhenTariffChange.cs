namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain;
using Domain.Configuration.Tariffs.Events;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts.Events;
using Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(TariffUpdated))]
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

    public async Task Handle(DomainEvent @event)
    {
        _logger.LogInformation("Refreshing appointments for tariff {TariffId}", @event.StreamId);
        var appointments = await _queryService.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.TypeId), @event.StreamId));

        foreach (var appointment in appointments)
        {
            await _eventStore.Persist<Appointment>(appointment);
        }
    }
}