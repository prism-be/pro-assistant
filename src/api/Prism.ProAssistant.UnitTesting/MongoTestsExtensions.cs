// -----------------------------------------------------------------------
//  <copyright file = "MongoTestsExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Moq;

namespace Prism.ProAssistant.UnitTesting;

public static class MongoTestsExtensions
{
    public static Mock<IMongoCollection<T>> SetupCollection<T>(this Mock<IMongoDatabase> database, params T[] samples) where T : new()
    {
        var collection = CreateCollection(samples);

        database.Setup(x => x.GetCollection<T>(typeof(T).Name.ToLowerInvariant(), null)).Returns(collection.Object);

        return collection;
    }

    public static Mock<IMongoCollection<T>> SetupCollectionAndReplace<T>(this Mock<IMongoDatabase> database, T replacement, params T[] samples) where T : new()
    {
        var collection = CreateCollection(samples);
        collection.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<T>>(), It.IsAny<T>(), It.IsAny<FindOneAndReplaceOptions<T, T>>(), CancellationToken.None))
            .ReturnsAsync(replacement);

        database.Setup(x => x.GetCollection<T>(typeof(T).Name.ToLowerInvariant(), null)).Returns(collection.Object);

        return collection;
    }

    private static Mock<IMongoCollection<T>> CreateCollection<T>(IReadOnlyCollection<T> samples) where T : new()
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