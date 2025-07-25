using MongoDB.Driver.GridFS;

namespace Prism.Infrastructure.Providers.Mongo;

using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDataStorage : IDataStorage
{
    private readonly IMongoClient _mongoClient;

    public MongoDataStorage(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task<Stream> CreateFileStreamAsync(string organization, string container, string fileName, string id)
    {
        var bucket = GetBucket(organization, container);
        return await bucket.OpenUploadStreamAsync(ObjectId.Parse(id), fileName);
    }

    public async Task<bool> ExistsAsync(string organization, string container, string id)
    {
        var bucket = GetBucket(organization, container);
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(id));
        return await (await bucket.FindAsync(filter)).AnyAsync();
    }

    public async Task<string> GetFileNameAsync(string organization, string container, string id)
    {
        var bucket = GetBucket(organization, container);
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(id));
        var fileInfo = await (await bucket.FindAsync(filter)).FirstOrDefaultAsync();
        return fileInfo.Filename;
    }

    public async Task<Stream> OpenFileStreamAsync(string organization, string container, string id)
    {
        var bucket = GetBucket(organization, container);
        return await bucket.OpenDownloadStreamAsync(ObjectId.Parse(id));
    }

    public async Task DeleteAsync(string organization, string container, string id)
    {
        var bucket = GetBucket(organization, container);
        await bucket.DeleteAsync(ObjectId.Parse(id));
    }

    private IGridFSBucket GetBucket(string organization, string container)
    {
        var db = _mongoClient.GetDatabase(organization);
        return new GridFSBucket(db, new GridFSBucketOptions
        {
            BucketName = container
        });
    }
}