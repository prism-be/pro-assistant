// -----------------------------------------------------------------------
//  <copyright file = "FindOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface IFindOneService
{
    Task<T?> FindOne<T>(string id);
}

public class FindOneService : IFindOneService
{
    private readonly IOrganizationContext _organizationContext;

    public FindOneService(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<T?> FindOne<T>(string id)
    {
        var collection = _organizationContext.GetCollection<T>();
        var query = await collection.FindAsync<T>(Builders<T>.Filter.Eq("Id", id));
        return await query.SingleOrDefaultAsync();
    }
}