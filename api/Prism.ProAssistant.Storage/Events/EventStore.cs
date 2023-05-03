namespace Prism.ProAssistant.Storage.Events;

using Core;
using Domain;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

public class EventStore : IEventStore, IHydrator
{
    private readonly ILogger<EventStore> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStateProvider _stateProvider;
    private readonly UserOrganization _userOrganization;
    private readonly IPublisher _publisher;

    public EventStore(ILogger<EventStore> logger, IStateProvider stateProvider, UserOrganization userOrganization, IServiceProvider serviceProvider, IPublisher publisher)
    {
        _logger = logger;
        _stateProvider = stateProvider;
        _userOrganization = userOrganization;
        _serviceProvider = serviceProvider;
        _publisher = publisher;
    }

    public async Task Raise(BaseEvent eventData)
    {
        var eventId = Identifier.GenerateString();
        _logger.LogInformation("Raising event {EventId} of type {EventType} for stream {StreamId}", eventId, eventData.GetType().Name, eventData.StreamId);

        var @event = DomainEvent.FromEvent(eventData.StreamId, _userOrganization.Id, eventData);

        var context = new EventContext
        {
            Context = _userOrganization,
            Event = @event
        };
        
        await _publisher.PublishAsync("domain/events", context);

        await Store(@event);
    }

    public async Task<UpsertResult> RaiseAndPersist<T>(BaseEvent eventData)
    {
        await Raise(eventData);
        return await Persist<T>(eventData.StreamId);
    }

    public async Task<UpsertResult> Persist<T>(string streamId)
    {
        _logger.LogInformation("Persisting state for stream {StreamId}", streamId);

        var state = await Hydrate<T>(streamId);

        var stateContainer = await _stateProvider.GetContainerAsync<T>();

        if (state == null)
        {
            await stateContainer.DeleteAsync(streamId);
        }
        else
        {
            await stateContainer.WriteAsync(streamId, state);
        }

        return new UpsertResult(streamId);
    }

    public async Task<T?> Hydrate<T>(string? streamId)
    {
        if (streamId == null)
        {
            return default;
        }

        _logger.LogDebug("Hydrating state for stream {StreamId}", streamId);
        var aggregator = GetAggregator<T>();
        var events = await GetEvents(streamId);

        aggregator.Init(streamId);

        foreach (var @event in events)
        {
            await aggregator.When(@event);
        }
        
        await aggregator.Complete();

        return aggregator.State;
    }

    private IDomainAggregator<T> GetAggregator<T>()
    {
        if (_serviceProvider.GetService(typeof(IDomainAggregator<T>)) is not IDomainAggregator<T> aggregator)
        {
            throw new NotSupportedException($"No aggregator found for type {typeof(T).Name}");
        }

        return aggregator;
    }

    private async Task<IEnumerable<DomainEvent>> GetEvents(string streamId)
    {
        var container = await _stateProvider.GetContainerAsync<DomainEvent>();
        var events = await container.FetchAsync(new Filter(nameof(DomainEvent.StreamId), streamId));
        return events.OrderBy(x => x.CreatedAt);
    }

    private async Task Store(DomainEvent @event)
    {
        var container = await _stateProvider.GetContainerAsync<DomainEvent>();
        await container.WriteAsync(@event.Id, @event);
    }
}