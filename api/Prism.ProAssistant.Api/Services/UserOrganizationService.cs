using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;

namespace Prism.ProAssistant.Api.Services;

public interface IUserOrganizationService
{
    Task<IMongoCollection<T>> GetUserCollection<T>()
        where T : IDataModel;

    Task<GridFSBucket> GetUserGridFsBucket();

    Task<string> GetUserOrganization();
}

public class UserOrganizationService : IUserOrganizationService
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserOrganizationService> _logger;
    private readonly IMongoClient _mongoClient;

    public UserOrganizationService(ILogger<UserOrganizationService> logger, IHttpContextAccessor httpContextAccessor, IMongoClient mongoClient, IDistributedCache cache)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _mongoClient = mongoClient;
        _cache = cache;
    }

    public async Task<IMongoCollection<T>> GetUserCollection<T>() where T : IDataModel
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var organization = await GetUserOrganization();
            var userDatabase = _mongoClient.GetDatabase(organization);
            var collectionName = GetCollectionName<T>();
            return userDatabase.GetCollection<T>(collectionName);
        }

        _logger.LogCritical("The collection was not found because the user is not authenticated.");
        throw new NotFoundException("The collection was not found because the user is not authenticated.");
    }

    public async Task<GridFSBucket> GetUserGridFsBucket()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var organization = await GetUserOrganization();
            var userDatabase = _mongoClient.GetDatabase(organization);
            return new GridFSBucket(userDatabase);
        }

        _logger.LogCritical("The collection was not found because the user is not authenticated.");
        throw new NotFoundException("The collection was not found because the user is not authenticated.");
    }

    public async Task<string> GetUserOrganization()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
        {
            return "demo";
        }

        var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new NotFoundException("The collection was not found because the user has no id.");
        }

        if (await _cache.GetAsync($"organization-{userId}") is { } organizationCached)
        {
            return Encoding.UTF8.GetString(organizationCached);
        }

        _logger.LogInformation("The user {userId} was not found in the cache, querying the database.", userId);

        var users = _mongoClient.GetDatabase("pro-assistant").GetCollection<BsonDocument>("users");
        var user = await (await users.FindAsync(x => x["userId"] == userId)).FirstOrDefaultAsync();

        if (user != null)
        {
            var organization = user["organization"].AsString;
            await _cache.SetStringAsync($"organization-{userId}", organization, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return organization;
        }

        _logger.LogWarning("The user {userId} was not found in the database, defaulting to the demo database.", userId);
        await users.InsertOneAsync(new BsonDocument(
            new[]
            {
                new KeyValuePair<string, object>("userId", userId),
                new KeyValuePair<string, object>("organization", "demo")
            }
        ));

        return "demo";
    }

    private static string GetCollectionName<T>()
    {
        var name = (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute)?.CollectionName;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NotImplementedException("The collection type is not implemented.");
        }

        return name;
    }
}