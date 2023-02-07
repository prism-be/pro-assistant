// -----------------------------------------------------------------------
//  <copyright file = "MongoTestsExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.UnitTesting;

public static class MongoTestsExtensions
{
    public static Mock<IGridFSBucket> SetupBucket(this Mock<IOrganizationContext> organizationContext, params GridFSFileInfo[] files)
    {
        var bucket = new Mock<IGridFSBucket>();

        organizationContext.Setup(x => x.GetGridFsBucket())
            .Returns(bucket.Object);

        var items = new List<GridFSFileInfo>();
        items.AddRange(files);
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

        bucket.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<GridFSFileInfo>>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);

        return bucket;
    }

    public static Mock<IMongoCollection<T>> SetupCollection<T>(this Mock<IOrganizationContext> organizationContext, params T[] samples)
    {
        var collection = CreateCollection(samples);

        organizationContext.Setup(x => x.GetCollection<T>())
            .Returns(collection.Object);
        
        return collection;
    }

    private static Mock<IMongoCollection<T>> CreateCollection<T>(IReadOnlyCollection<T> samples)
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

        var collection = new Mock<IMongoCollection<T>>();
        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                CancellationToken.None))
            .ReturnsAsync(cursor.Object);
        collection.Object.InsertMany(items);
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<T>.Empty, null, CancellationToken.None)).ReturnsAsync(samples.Count);
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<T>>(), CancellationToken.None)).ReturnsAsync(new DeleteResult.Acknowledged(1));
        return collection;
    }
}