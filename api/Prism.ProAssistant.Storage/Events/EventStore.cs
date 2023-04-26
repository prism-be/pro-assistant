namespace Prism.ProAssistant.Storage.Events;

using Core;
using Domain;
using Domain.Configuration.DocumentConfiguration;
using Domain.Configuration.Settings;
using Domain.Configuration.Tariffs;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

public interface IEventStore
{
    Task<T?> Hydrate<T>(string streamId);
    Task<UpsertResult> Persist<T>(string streamId);
    Task Raise(IDomainEvent eventData);
    Task<UpsertResult> RaiseAndPersist<T>(IDomainEvent eventData);
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

    public async Task<UpsertResult> RaiseAndPersist<T>(IDomainEvent eventData)
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

    public async Task<T?> Hydrate<T>(string streamId)
    {
        _logger.LogDebug("Hydrating state for stream {StreamId}", streamId);
        var aggregator = GetAggregator<T>();
        var events = await GetEvents(streamId);

        aggregator.Init(streamId);

        foreach (var @event in events)
        {
            aggregator.When(@event);
        }

        return aggregator.State;
    }

    private static IDomainAggregator<T> GetAggregator<T>()
    {
        return typeof(T).Name switch
        {
            nameof(Appointment) => (IDomainAggregator<T>)new AppointmentAggregator(),
            nameof(Contact) => (IDomainAggregator<T>)new ContactAggregator(),
            nameof(DocumentConfiguration) => (IDomainAggregator<T>)new DocumentConfigurationAggregator(),
            nameof(Setting) => (IDomainAggregator<T>)new SettingAggregator(),
            nameof(Tariff) => (IDomainAggregator<T>)new TariffAggregator(),
            _ => throw new NotSupportedException($"No aggregator found for type {typeof(T).Name}")
        };
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