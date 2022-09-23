// -----------------------------------------------------------------------
//  <copyright file = "MongoDatabaseExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MongoDB.Driver;

public static class MongoDatabaseExtensions
{
    public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database)
    {
        var collectionName = typeof(T).Name.ToLowerInvariant();
        return database.GetCollection<T>(collectionName);
    }
}