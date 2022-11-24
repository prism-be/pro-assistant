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
    IMongoCollection<T> GetCollection<T>();
}

public class OrganizationContext : IOrganizationContext
{
    private readonly IMongoDatabase _database;

    public OrganizationContext(IMongoClient client, IUserContextAccessor userContextAccessor)
    {
        _database = string.IsNullOrWhiteSpace(userContextAccessor.OrganisationId)
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

        return _database.GetCollection<T>(collectionName);
    }

    private static string? GetCollectionName<T>()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
            as BsonCollectionAttribute)?.CollectionName;
    }
}