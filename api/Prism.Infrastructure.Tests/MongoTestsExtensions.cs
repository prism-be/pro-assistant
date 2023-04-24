using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;

namespace Prism.Infrastructure.Tests;

public static class MongoTestsExtensions
{

    public static void SetupBucket(this Mock<IGridFSBucket> collection, params GridFSFileInfo[] samples)
    {
        var items = new List<GridFSFileInfo>();
        items.AddRange(samples);
        var cursor = new Mock<IAsyncCursor<GridFSFileInfo>>();
        cursor.Setup(_ => _.Current).Returns(items);
        cursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<GridFSFileInfo>>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);
    }

    public static void SetupCollection<T>(this Mock<IMongoCollection<T>> collection, params T[] samples)
    {
        CreateCollection(collection, samples);
    }

    private static void CreateCollection<T>(Mock<IMongoCollection<T>> collection, IReadOnlyCollection<T> samples)
    {
        var items = new List<T>();
        items.AddRange(samples);
        var cursor = new Mock<IAsyncCursor<T>>();
        cursor.Setup(_ => _.Current).Returns(items);
        cursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                CancellationToken.None))
            .ReturnsAsync(cursor.Object);
        collection.Object.InsertMany(items);
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<T>.Empty, null, CancellationToken.None)).ReturnsAsync(samples.Count);
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<T>>(), CancellationToken.None)).ReturnsAsync(new DeleteResult.Acknowledged(1));
    }
}