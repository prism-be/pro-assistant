// -----------------------------------------------------------------------
//  <copyright file = "RemoveOneService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface IRemoveOneService
{
    Task Remove<T>(string id)
        where T : IDataModel;
}

public class RemoveOneService : IRemoveOneService
{
    private readonly ILogger<RemoveOneService> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly User _user;

    public RemoveOneService(ILogger<RemoveOneService> logger, IOrganizationContext organizationContext, User user)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _user = user;
    }

    public async Task Remove<T>(string id)
        where T : IDataModel
    {
        await _logger.LogDataDelete(_user, id, async () =>
        {
            var collection = _organizationContext.GetCollection<T>();
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
        });
    }
}