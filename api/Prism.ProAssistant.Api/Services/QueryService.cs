using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IQueryService
{
    Task<List<T>> ListAsync<T>() where T : IDataModel;
    Task<List<T>> SearchAsync<T>(List<SearchFilter> request) where T : IDataModel;
    Task<T> SingleAsync<T>(string id) where T : IDataModel;
    Task<T?> SingleOrDefaultAsync<T>(string id) where T : IDataModel;
}

public class QueryService : IQueryService
{
    private readonly ILogger<QueryService> _logger;
    private readonly IUserOrganizationService _userOrganizationService;

    public QueryService(IUserOrganizationService userOrganizationService, ILogger<QueryService> logger)
    {
        _userOrganizationService = userOrganizationService;
        _logger = logger;
    }

    public async Task<List<T>> ListAsync<T>() where T : IDataModel
    {
        _logger.LogInformation("ListAsync - {Type} - {UserId}", typeof(T).Name, _userOrganizationService.GetUserId());

        var collection = await _userOrganizationService.GetUserCollection<T>();
        var query = await collection.FindAsync<T>(Builders<T>.Filter.Empty);
        return await query.ToListAsync();
    }

    public async Task<T> SingleAsync<T>(string id) where T : IDataModel
    {
        _logger.LogInformation("SingleAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganizationService.GetUserId());

        var collection = await _userOrganizationService.GetUserCollection<T>();
        IAsyncCursor<T?> query = await collection.FindAsync<T>(Builders<T>.Filter.Eq(x => x.Id, id));
        var result = await query.SingleOrDefaultAsync();

        if (result == null)
        {
            throw new NotFoundException($"Document not found in collection {typeof(T).Name} with id {id}");
        }

        return result;
    }

    public async Task<T?> SingleOrDefaultAsync<T>(string id) where T : IDataModel
    {
        _logger.LogInformation("SingleOrDefaultAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganizationService.GetUserId());

        if (id == "000000000000000000000000")
        {
            return default;
        }

        var collection = await _userOrganizationService.GetUserCollection<T>();
        IAsyncCursor<T?> query = await collection.FindAsync<T>(Builders<T>.Filter.Eq(x => x.Id, id));
        return await query.SingleOrDefaultAsync();
    }

    public async Task<List<T>> SearchAsync<T>(List<SearchFilter> request) where T : IDataModel
    {
        _logger.LogInformation("SearchAsync - {Type} - {UserId}", typeof(T).Name, _userOrganizationService.GetUserId());

        var query = Builders<T>.Filter.Empty;

        foreach (var filter in request)
        {
            if (DateTime.TryParse(filter.Value.ToString(), out var date))
            {
                filter.Value = date;
            }

            switch (filter.Operator)
            {
                case "eq":
                    query &= Builders<T>.Filter.Eq(filter.Field, filter.Value);
                    break;
                case "ne":
                    query &= Builders<T>.Filter.Ne(filter.Field, filter.Value);
                    break;
                case "gt":
                    query &= Builders<T>.Filter.Gt(filter.Field, filter.Value);
                    break;
                case "gte":
                    query &= Builders<T>.Filter.Gte(filter.Field, filter.Value);
                    break;
                case "lt":
                    query &= Builders<T>.Filter.Lt(filter.Field, filter.Value);
                    break;
                case "lte":
                    query &= Builders<T>.Filter.Lte(filter.Field, filter.Value);
                    break;
                case "regex":
                    query &= Builders<T>.Filter.Regex(filter.Field, BsonRegularExpression.Create(new Regex(filter.Value.ToString() ?? throw new InvalidOperationException("Value is null for regex filter"), RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100))));
                    break;
                default:
                    throw new NotSupportedException("Filter type not supported: " + filter.Operator);
            }
        }

        var collection = await _userOrganizationService.GetUserCollection<T>();
        var items = await collection.FindAsync(query);
        return await items.ToListAsync();
    }
}