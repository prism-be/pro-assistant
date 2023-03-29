using System.Text.Json;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IEventAggregator
{
    Task AggregateAsync<T>(string objectId) where T : IDataModel;
}

public class EventAggregator : IEventAggregator
{
    private readonly ILogger<EventAggregator> _logger;
    private readonly IUserOrganizationService _userOrganizationService;

    public EventAggregator(IUserOrganizationService userOrganizationService, ILogger<EventAggregator> logger)
    {
        _userOrganizationService = userOrganizationService;
        _logger = logger;
    }

    public async Task AggregateAsync<T>(string objectId) where T : IDataModel
    {
        _logger.LogInformation("AggregateAsync - {Id} - {Type} - {UserId}", objectId, typeof(T).Name, _userOrganizationService.GetUserId());
        var eventsCollection = await _userOrganizationService.GetUserEventCollection<T>();

        var events = await eventsCollection.FindAsync(Builders<Event<T>>.Filter.Eq(
                x => x.ObjectId, objectId),
            new FindOptions<Event<T>, Event<T>>
            {
                Sort = Builders<Event<T>>.Sort.Ascending(x => x.CreatedAt)
            }
        );

        var item = default(T);

        while (await events.MoveNextAsync())
        {
            foreach (var e in events.Current)
            {
                item = Apply(item, e);
            }
        }

        var collection = await _userOrganizationService.GetUserCollection<T>();

        if (item == null)
        {
            _logger.LogInformation("AggregateAsync - {Id} - {Type} - {UserId} - Deleting", objectId, typeof(T).Name, _userOrganizationService.GetUserId());
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, objectId));
        }
        else
        {
            _logger.LogInformation("AggregateAsync - {Id} - {Type} - {UserId} - Upserting", objectId, typeof(T).Name, _userOrganizationService.GetUserId());
            await collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, objectId), item, new ReplaceOptions { IsUpsert = true });
        }
    }

    private T? Apply<T>(T? item, Event<T> e) where T : IDataModel
    {
        switch (e.EventType)
        {
            case EventType.Insert:
            case EventType.Replace:
                return e.Data;
            case EventType.Delete:
                return default;
            case EventType.Update:
                if (item == null)
                {
                    throw new InvalidOperationException("Cannot update a null item.");
                }
                
                if (e.Updates == null)
                {
                    throw new InvalidOperationException("Cannot apply null updates.");
                }

                foreach (var update in e.Updates)
                {
                    var property = item.GetType().GetProperty(update.Key);
                    if (property == null)
                    {
                        throw new InvalidOperationException($"Property {update.Key} does not exist on type {typeof(T).Name}.");
                    }

                    var value = JsonSerializer.Deserialize(update.Value, property.PropertyType);
                    property.SetValue(item, value);
                }

                return item;
            default:
                throw new NotSupportedException($"Event type {e.EventType} is not supported.");
        }
    }
}