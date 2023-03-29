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
                switch (e.EventType)
                {
                    case EventType.Insert:
                        item = ApplyInsert(e);
                        break;
                    default:
                        throw new NotSupportedException($"Event type {e.EventType} is not supported.");
                }
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

    private static T? ApplyInsert<T>(Event<T> e) where T : IDataModel
    {
        return e.Data;
    }
}