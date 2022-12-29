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
    IMongoDatabase Database { get; }

    string OrganizationId { get; }

    IMongoCollection<T> GetCollection<T>();

    void SelectOrganization(string organizationId);
    
    IGridFSBucket GetGridFsBucket();
}

public class OrganizationContext : IOrganizationContext
{
    private readonly IMongoClient _client;

    public OrganizationContext(IMongoClient client, IUserContextAccessor userContextAccessor)
    {
        _client = client;

        OrganizationId = string.IsNullOrWhiteSpace(userContextAccessor.OrganizationId)
            ? "unknown"
            : userContextAccessor.OrganizationId;

        Database = client.GetDatabase(OrganizationId);
    }

    public string OrganizationId { get; private set; }

    public IMongoDatabase Database { get; private set; }

    public IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = GetCollectionName<T>();

        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new NotSupportedException("The type is not supported as a data model : " + typeof(T).FullName);
        }

        return Database.GetCollection<T>(collectionName);
    }

    public void SelectOrganization(string organizationId)
    {
        OrganizationId = organizationId;
        Database = _client.GetDatabase(organizationId);
    }

    public IGridFSBucket GetGridFsBucket()
    {
        return new GridFSBucket(this.Database);
    }

    private static string? GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute)?.CollectionName;
    }
}