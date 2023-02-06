// -----------------------------------------------------------------------
//  <copyright file = "RemoveOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public interface IRemoveOneService
{
    Task Remove<T>(string organizationId, string userId, string id)
        where T : IDataModel;
}

public class RemoveOneService : IRemoveOneService
{
    private readonly ILogger<RemoveOneService> _logger;

    private readonly IOrganizationContext _organizationContext;

    public RemoveOneService(ILogger<RemoveOneService> logger, IOrganizationContext organizationContext)
    {
        _logger = logger;
        _organizationContext = organizationContext;
    }

    public async Task Remove<T>(string organizationId, string userId, string id)
        where T : IDataModel
    {
        await _logger.LogDataDelete(organizationId, userId, id, async () =>
        {
            _organizationContext.SelectOrganization(organizationId);
            var collection = _organizationContext.GetCollection<T>();
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
        });
    }
}