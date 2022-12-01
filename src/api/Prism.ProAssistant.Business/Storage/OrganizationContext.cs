// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Storage;

public interface IOrganizationContext
{
    IMongoDatabase Database { get; }
    
    IMongoCollection<T> GetCollection<T>();
}

public class OrganizationContext : IOrganizationContext
{
    public IMongoDatabase Database { get; }

    public OrganizationContext(IMongoClient client, IUserContextAccessor userContextAccessor)
    {
        Database = string.IsNullOrWhiteSpace(userContextAccessor.OrganisationId)
            ? client.GetDatabase("unknown")
            : client.GetDatabase(userContextAccessor.OrganisationId);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = GetCollectionName<T>();

        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new NotSupportedException("The type is not supported as a data model : " + typeof(T).FullName);
        }

        return Database.GetCollection<T>(collectionName);
    }

    private static string? GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute)?.CollectionName;
    }
}