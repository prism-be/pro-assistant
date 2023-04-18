using Microsoft.Extensions.Logging;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Contacts;

namespace Prism.ProAssistant.Storage.Events;

public interface IEventStore
{
    Task Raise(IDomainEvent eventData);
    Task<UpsertResult> RaiseAndPersist<T>(IDomainEvent eventData);
}

public class EventStore : IEventStore
{
    private readonly ILogger<EventStore> _logger;
    private readonly IStateProvider _stateProvider;
    private readonly UserOrganization _userOrganization;

    private static IDomainAggregator<T> GetAggregator<T>()
    {
        return typeof(T).Name switch
        {
            nameof(Appointment) => (IDomainAggregator<T>)new AppointmentAggregator(),
            nameof(Contact) => (IDomainAggregator<T>)new ContactAggregator(),
            _ => throw new NotSupportedException($"No aggregator found for type {typeof(T).Name}")
        };
    }

    public EventStore(ILogger<EventStore> logger, IStateProvider stateProvider, UserOrganization userOrganization)
    {
        _logger = logger;
        _stateProvider = stateProvider;
        _userOrganization = userOrganization;
    }

    public async Task Raise(IDomainEvent eventData)
    {
        var eventId = Identifier.GenerateString();
        _logger.LogInformation("Raising event {EventId} of type {EventType} for stream {StreamId}", eventId, eventData.GetType().Name, eventData.StreamId);

        var @event = DomainEvent.FromEvent(eventData.StreamId, _userOrganization.Id, eventData);

        await Store(@event);
    }

    public async Task<UpsertResult> RaiseAndPersist<T>(IDomainEvent eventData)
    {
        await Raise(eventData);

        _logger.LogInformation("Persisting state for stream {StreamId}", eventData.StreamId);
        var aggregator = GetAggregator<T>();
        var events = await GetEvents(eventData.StreamId);

        aggregator.Init(eventData.StreamId);

        foreach (var @event in events)
        {
            aggregator.When(@event);
        }

        var stateContainer = await _stateProvider.GetContainerAsync<T>();
        await stateContainer.WriteAsync(eventData.StreamId, aggregator.State);
        
        return new UpsertResult(eventData.StreamId);
    }

    private async Task<IEnumerable<DomainEvent>> GetEvents(string streamId)
    {
        var container = await _stateProvider.GetContainerAsync<DomainEvent>();
        return await container.FetchAsync(new Filter(nameof(DomainEvent.StreamId), streamId));
    }

    private async Task Store(DomainEvent @event)
    {
        var container = await _stateProvider.GetContainerAsync<DomainEvent>();
        await container.WriteAsync(@event.Id, @event);
    }
}