// -----------------------------------------------------------------------
//  <copyright file = "TestsExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using HotChocolate.Data;
using MongoDB.Driver;
using Moq;

namespace Prism.ProAssistant.Api.Tests;

public static class MongoTestsExtensions
{
    public static void SetupCollectionFindEmpty<T>(this Mock<IMongoDatabase> database, params T[] samples)
    {
        var items = new List<T>();
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
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<T>.Empty, null, CancellationToken.None)).ReturnsAsync(samples.Length);
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<T>>(), CancellationToken.None)).ReturnsAsync(new DeleteResult.Acknowledged(1));
        
        database.Setup(x => x.GetCollection<T>(typeof(T).Name.ToLowerInvariant(), null)).Returns(collection.Object);
    }
    
    public static void SetupCollection<T>(this Mock<IMongoDatabase> database, params T[] samples)
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
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<T>.Empty, null, CancellationToken.None)).ReturnsAsync(samples.Length);
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<T>>(), CancellationToken.None)).ReturnsAsync(new DeleteResult.Acknowledged(1));
        
        database.Setup(x => x.GetCollection<T>(typeof(T).Name.ToLowerInvariant(), null)).Returns(collection.Object);
    }
}