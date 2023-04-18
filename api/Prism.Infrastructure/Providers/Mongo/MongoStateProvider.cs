using MongoDB.Bson;
using MongoDB.Driver;
using Prism.Core.Attributes;
using Prism.Infrastructure.Authentication;

namespace Prism.Infrastructure.Providers.Mongo;

public class MongoStateProvider : IStateProvider
{
    private readonly IMongoClient _mongoClient;
    private readonly UserOrganization _userOrganization;

    public MongoStateProvider(IMongoClient mongoClient, UserOrganization userOrganization)
    {
        _mongoClient = mongoClient;
        _userOrganization = userOrganization;
    }

    public Task<IStateContainer<T>> GetContainerAsync<T>()
    {
        var db = _mongoClient.GetDatabase(_userOrganization.Organization);
        var collection = CollectionAttribute.GetCollectionName<T>();
        var container = db.GetCollection<T>(collection);
        return Task.FromResult(new MongoStateContainer<T>(container) as IStateContainer<T>);
    }

    public string GenerateUniqueIdentifier()
    {
        return ObjectId.GenerateNewId().ToString();
    }
}