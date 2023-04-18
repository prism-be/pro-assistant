using MongoDB.Driver;

namespace Prism.Infrastructure.Providers.Mongo;

public class MongoStateContainer<T> : IStateContainer<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoStateContainer(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public async Task<T?> ReadAsync(string id)
    {
        var result = await _collection.FindAsync(Builders<T>.Filter.Eq("Id", id));
        return await result.SingleOrDefaultAsync();
    }

    public Task WriteAsync(string id, T value)
    {
        return _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), value, new ReplaceOptions { IsUpsert = true });
    }

    public async Task<IEnumerable<T>> FetchAsync(params Filter[] filters)
    {
        var filter = BuildFilter(filters);
        var results = await _collection.FindAsync(filter);
        return results.ToEnumerable();
    }

    private static FilterDefinition<T> BuildFilter(Filter[] filters)
    {
        if (filters.Length == 0)
        {
            return Builders<T>.Filter.Empty;
        }

        return Builders<T>.Filter.And(filters.Select(f =>
        {
            switch (f.Op)
            {
                case FilterOperator.Equal:
                    return Builders<T>.Filter.Eq(f.Property, f.Value);
            }

            throw new NotSupportedException($"Filter operator {f.Op} not supported");
        }));
    }
}