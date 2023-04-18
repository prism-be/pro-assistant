﻿using System.Text.Json;
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
    private readonly IStateProvider _stateProvider;
    private readonly UserOrganization _userOrganization;

    public EventStore(IStateProvider stateProvider, UserOrganization userOrganization)
    {
        _stateProvider = stateProvider;
        _userOrganization = userOrganization;
    }

    public async Task Raise(IDomainEvent eventData)
    {
        var @event = new DomainEvent
        {
            Id = _stateProvider.GenerateUniqueIdentifier(),
            Type = eventData.GetType().Name ?? throw new InvalidOperationException(),
            StreamId = eventData.StreamId,
            Data = JsonSerializer.Serialize(eventData as object),
            UserId = _userOrganization.Id
        };

        await Store(@event);
    }

    public async Task RaiseAndPersist<TAggregator, T>(IDomainEvent eventData)
        where TAggregator : IDomainAggregator<T>, new()
    {
        await Raise(eventData);

        var aggregator = new TAggregator();
        var events = await GetEvents<T>(eventData.StreamId);

        foreach (var @event in events)
        {
            aggregator.When(@event);
        }

        var stateContainer = await _stateProvider.GetContainerAsync<T>();
        await stateContainer.WriteAsync(eventData.StreamId, aggregator.State);
    }

    private async Task<IEnumerable<DomainEvent>> GetEvents<T>(string streamId)
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