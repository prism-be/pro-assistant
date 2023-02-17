// -----------------------------------------------------------------------
//  <copyright file = "FindManyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface IFindManyService
{
    Task<List<T>> Find<T>();
    Task<List<T>> Find<T>(FilterDefinition<T> filter);
}

public class FindManyService : IFindManyService
{
    private readonly IOrganizationContext _organizationContext;

    public FindManyService(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<T>> Find<T>()
    {
        var collection = _organizationContext.GetCollection<T>();
        var results = await collection.FindAsync<T>(Builders<T>.Filter.Empty);
        return results.ToList();
    }

    public async Task<List<T>> Find<T>(FilterDefinition<T> filter)
    {
        var collection = _organizationContext.GetCollection<T>();
        var results = await collection.FindAsync<T>(filter);
        return results.ToList();
    }
}