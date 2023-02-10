// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Storage;

public interface IOrganizationContext
{
    IMongoCollection<T> GetCollection<T>();

    IGridFSBucket GetGridFsBucket();
}

public class OrganizationContext : IOrganizationContext
{
    private readonly IMongoClient _client;
    private readonly User _user;

    public OrganizationContext(IMongoClient client, User user)
    {
        _client = client;
        _user = user;
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = GetCollectionName<T>();

        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new NotSupportedException("The type is not supported as a data model : " + typeof(T).FullName);
        }

        var database = _client.GetDatabase(_user.Organization);
        return database.GetCollection<T>(collectionName);
    }

    public IGridFSBucket GetGridFsBucket()
    {
        var database = _client.GetDatabase(_user.Organization);
        return new GridFSBucket(database);
    }

    private static string? GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute)?.CollectionName;
    }
}