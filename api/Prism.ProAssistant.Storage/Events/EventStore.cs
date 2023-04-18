using Microsoft.Extensions.Logging;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain;

namespace Prism.ProAssistant.Storage.Events;

public interface IEventStore
{
    Task Raise(IDomainEvent eventData);
    Task RaiseAndPersist<TAggregator, T>(IDomainEvent eventData) where TAggregator : IDomainAggregator<T>, new();
}

public class EventStore : IEventStore
{
    private readonly ILogger<EventStore> _logger;
    private readonly IStateProvider _stateProvider;
    private readonly UserOrganization _userOrganization;

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

    public async Task RaiseAndPersist<TAggregator, T>(IDomainEvent eventData)
        where TAggregator : IDomainAggregator<T>, new()
    {
        await Raise(eventData);

        _logger.LogInformation("Persisting state for stream {StreamId}", eventData.StreamId);
        var aggregator = new TAggregator();
        var events = await GetEvents(eventData.StreamId);

        aggregator.Init(eventData.StreamId);

        foreach (var @event in events)
        {
            aggregator.When(@event);
        }

        var stateContainer = await _stateProvider.GetContainerAsync<T>();
        await stateContainer.WriteAsync(eventData.StreamId, aggregator.State);
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