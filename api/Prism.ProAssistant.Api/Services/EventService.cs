using System.Text.Json;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IEventService
{
    Task<UpsertResult> CreateAsync<T>(T data) where T : IDataModel;
    Task<bool> DeleteAsync<T>(string id) where T : IDataModel;
    Task<UpsertResult> ReplaceAsync<T>(T item) where T : IDataModel;
    Task<UpsertResult> UpdateAsync<T>(string id, params KeyValuePair<string, object>[] updates) where T : IDataModel;
}

public class EventService : IEventService
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger<EventService> _logger;
    private readonly IUserOrganizationService _userOrganizationService;

    public EventService(IUserOrganizationService userOrganizationService, ILogger<EventService> logger, IEventAggregator eventAggregator)
    {
        _userOrganizationService = userOrganizationService;
        _logger = logger;
        _eventAggregator = eventAggregator;
    }

    public async Task<UpsertResult> CreateAsync<T>(T data) where T : IDataModel
    {
        data.Id = Identifier.GenerateString();

        _logger.LogInformation("InsertAsync - {Id} - {Type} - {UserId}", data.Id, typeof(T).Name, _userOrganizationService.GetUserId());

        var e = new Event<T>
        {
            ObjectId = data.Id,
            EventType = EventType.Insert,
            Data = data,
            UserId = _userOrganizationService.GetUserId()
        };

        await Save(e);
        await _eventAggregator.AggregateAsync<T>(data.Id);

        return new UpsertResult(data.Id);
    }

    public async Task<bool> DeleteAsync<T>(string id) where T : IDataModel
    {
        _logger.LogInformation("DeleteAsync - {Id} - {Type} - {UserId}", id, typeof(T).Name, _userOrganizationService.GetUserId());

        var e = new Event<T>
        {
            ObjectId = id,
            EventType = EventType.Delete,
            UserId = _userOrganizationService.GetUserId()
        };

        await Save(e);
        await _eventAggregator.AggregateAsync<T>(id);

        return true;
    }

    public async Task<UpsertResult> ReplaceAsync<T>(T item) where T : IDataModel
    {
        _logger.LogInformation("ReplaceAsync - {Id} - {Type} - {UserId}", item.Id, typeof(T).Name, _userOrganizationService.GetUserId());

        var e = new Event<T>
        {
            ObjectId = item.Id,
            EventType = EventType.Replace,
            Data = item,
            UserId = _userOrganizationService.GetUserId()
        };

        await Save(e);
        await _eventAggregator.AggregateAsync<T>(item.Id);

        return new UpsertResult(item.Id);
    }

    public async Task<UpsertResult> UpdateAsync<T>(string id, params KeyValuePair<string, object>[] updates) where T : IDataModel
    {
        _logger.LogInformation("UpdateAsync - {Id} - {Type} - {UserId}", id, typeof(T).Name, _userOrganizationService.GetUserId());

        var e = new Event<T>
        {
            ObjectId = id,
            EventType = EventType.Update,
            Updates = updates.Select(x => new KeyValuePair<string, string>(x.Key, JsonSerializer.Serialize(x.Value))).ToArray(),
            UserId = _userOrganizationService.GetUserId()
        };

        await Save(e);
        await _eventAggregator.AggregateAsync<T>(id);
        
        return new UpsertResult(id);
    }

    private async Task Save<T>(Event<T> e) where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserEventCollection<T>();
        await collection.InsertOneAsync(e);
    }
}