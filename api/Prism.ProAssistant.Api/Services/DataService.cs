using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IDataService
{
    Task DeleteFileAsync(string id);
    Task<byte[]?> GetFileAsync(string id);
    Task<string> GetFileNameAsync(string id);
    Task<List<T>> ListAsync<T>() where T : IDataModel;
    Task<List<UpsertResult>> ReplaceManyAsync<T>(List<T> request) where T : IDataModel;
    Task<List<T>> SearchAsync<T>(List<SearchFilter> request) where T : IDataModel;
    Task<T> SingleAsync<T>(string id) where T : IDataModel;
    Task<T?> SingleOrDefaultAsync<T>(string id) where T : IDataModel;
    Task<UpsertResult> UpdateAsync<T>(T request, params string[] properties) where T : IDataModel;
    Task UpdateManyAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : IDataModel;
    Task<string> UploadFromBytesAsync(string fileName, byte[] bytes);
}

public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;
    private readonly IUserOrganizationService _userOrganizationService;

    public DataService(IUserOrganizationService userOrganizationService, ILogger<DataService> logger)
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

    public async Task<UpsertResult> UpdateAsync<T>(T request, params string[] properties) where T : IDataModel
    {
        _logger.LogInformation("UpdateAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, request.Id, _userOrganizationService.GetUserId());

        var collection = await _userOrganizationService.GetUserCollection<T>();

        var updates = (from property in properties
            let value = request.GetType().GetProperty(property)?.GetValue(request)
            select Builders<T>.Update.Set(property, value)).ToList();

        await collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, request.Id), Builders<T>.Update.Combine(updates));

        return new UpsertResult(request.Id);
    }

    public async Task<List<UpsertResult>> ReplaceManyAsync<T>(List<T> request) where T : IDataModel
    {
        _logger.LogInformation("ReplaceManyAsync - {Type} - {UserId}", typeof(T).Name, _userOrganizationService.GetUserId());

        var results = new List<UpsertResult>();
        var collection = await _userOrganizationService.GetUserCollection<T>();

        foreach (var item in request)
        {
            _logger.LogInformation("ReplaceManyAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, item.Id, _userOrganizationService.GetUserId());

            var replaced = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq(x => x.Id, item.Id), item);

            if (replaced == null)
            {
                await collection.InsertOneAsync(item);
            }

            results.Add(new UpsertResult(item.Id));
        }

        return results;
    }

    public async Task UpdateManyAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : IDataModel
    {
        _logger.LogInformation("UpdateManyAsync - {Type} - {UserId}", typeof(T).Name, _userOrganizationService.GetUserId());

        var collection = await _userOrganizationService.GetUserCollection<T>();
        await collection.UpdateManyAsync(filter, update);
    }

    public async Task<string> UploadFromBytesAsync(string fileName, byte[] bytes)
    {
        _logger.LogInformation("UploadFromBytesAsync - {FileName} - {UserId}", fileName, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        var id = await bucket.UploadFromBytesAsync(fileName, bytes);
        return id.ToString();
    }

    public async Task DeleteFileAsync(string id)
    {
        _logger.LogInformation("DeleteFileAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        await bucket.DeleteAsync(new ObjectId(id));
    }

    public async Task<byte[]?> GetFileAsync(string id)
    {
        _logger.LogInformation("GetFileAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        return await bucket.DownloadAsBytesAsync(new ObjectId(id));
    }

    public async Task<string> GetFileNameAsync(string id)
    {
        _logger.LogInformation("GetFileNameAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        var result = await bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, new ObjectId(id)));
        return await result.SingleAsync().ContinueWith(x => x.Result.Filename);
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