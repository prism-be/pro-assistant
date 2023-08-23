namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

[SideEffect(typeof(Contact))]
public class RefreshAppointmentWhenContactChange
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<RefreshAppointmentWhenContactChange> _logger;
    private readonly IQueryService _queryService;

    public RefreshAppointmentWhenContactChange(ILogger<RefreshAppointmentWhenContactChange> logger, IQueryService queryService, IEventStore eventStore)
    {
        _logger = logger;
        _queryService = queryService;
        _eventStore = eventStore;
    }

    public async Task Handle(EventContext<Contact> context)
    {
        if (context.CurrentState == null)
        {
            return;
        }
        
        _logger.LogInformation("Refreshing appointments for contact {ContactId}", context.Event.StreamId);
        var appointments = await _queryService.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.ContactId), context.CurrentState.Id));

        foreach (var appointment in appointments)
        {
            await _eventStore.Persist<Appointment>(appointment);
        }
    }
}