using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IEventService
{
    Task<UpsertResult> CreateAsync<T>(T data) where T : IDataModel;
}

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly IUserOrganizationService _userOrganizationService;
    private readonly IEventAggregator _eventAggregator;

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

    private async Task Save<T>(Event<T> e) where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserEventCollection<T>();
        await collection.InsertOneAsync(e);
    }
}