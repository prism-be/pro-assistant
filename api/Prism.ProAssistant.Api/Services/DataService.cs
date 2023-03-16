using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IDataService
{

    Task<UpsertResult> InsertAsync<T>(T request) where T : IDataModel;

    Task<List<T>> ListAsync<T>()
        where T : IDataModel;

    Task<List<T>> SearchAsync<T>(List<SearchFilter> request)
        where T : IDataModel;

    Task<T?> SingleAsync<T>(string id)
        where T : IDataModel;

    Task<UpsertResult> UpdateAsync<T>(T request)
        where T : IDataModel;

    Task<List<UpsertResult>> UpdateManyAsync<T>(List<T> request) where T : IDataModel;
    Task UpdateAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : IDataModel;
}

public class DataService : IDataService
{
    private readonly IUserOrganizationService _userOrganizationService;

    public DataService(IUserOrganizationService userOrganizationService)
    {
        _userOrganizationService = userOrganizationService;
    }

    public async Task<List<T>> ListAsync<T>() where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserCollection<T>();
        var query = await collection.FindAsync<T>(Builders<T>.Filter.Empty);
        return await query.ToListAsync();
    }

    public async Task<T?> SingleAsync<T>(string id) where T : IDataModel
    {
        if (id == "000000000000000000000000")
        {
            return default;
        }

        var collection = await _userOrganizationService.GetUserCollection<T>();
        IAsyncCursor<T?> query = await collection.FindAsync<T>(Builders<T>.Filter.Eq(x => x.Id, id));
        return await query.SingleAsync();
    }

    public async Task<UpsertResult> UpdateAsync<T>(T request) where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserCollection<T>();
        request = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq(x => x.Id, request.Id), request);
        return new UpsertResult(request.Id);
    }

    public async Task<List<UpsertResult>> UpdateManyAsync<T>(List<T> request) where T : IDataModel
    {
        var results = new List<UpsertResult>();
        var collection = await _userOrganizationService.GetUserCollection<T>();

        foreach (var item in request)
        {
            var replaced = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq(x => x.Id, item.Id), item);
            results.Add(new UpsertResult(replaced.Id));
        }

        return results;
    }

    public async Task UpdateAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserCollection<T>();
        await collection.UpdateManyAsync(filter, update);
    }

    public async Task<UpsertResult> InsertAsync<T>(T request) where T : IDataModel
    {
        var collection = await _userOrganizationService.GetUserCollection<T>();
        await collection.InsertOneAsync(request);
        return new UpsertResult(request.Id);
    }

    public async Task<List<T>> SearchAsync<T>(List<SearchFilter> request) where T : IDataModel
    {
        var query = Builders<T>.Filter.Empty;

        foreach (var filter in request)
        {
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
                    throw new NotImplementedException("Filter type not implemented: " + filter.Operator);
            }
        }

        var collection = await _userOrganizationService.GetUserCollection<T>();
        var items = await collection.FindAsync(query);
        return await items.ToListAsync();
    }
}