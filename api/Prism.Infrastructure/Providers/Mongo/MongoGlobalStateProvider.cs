using MongoDB.Driver;
using Prism.Core.Attributes;

namespace Prism.Infrastructure.Providers.Mongo;

public class MongoGlobalStateProvider : IGlobalStateProvider
{
    private readonly IMongoClient _mongoClient;

    public MongoGlobalStateProvider(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public Task<IStateContainer<T>> GetGlobalContainerAsync<T>()
    {
        var database = _mongoClient.GetDatabase("_global");
        var collection = CollectionAttribute.GetCollectionName<T>();
        var container = database.GetCollection<T>(collection);
        return Task.FromResult(new MongoStateContainer<T>(container) as IStateContainer<T>);
    }
}