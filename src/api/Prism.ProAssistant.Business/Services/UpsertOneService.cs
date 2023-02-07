// -----------------------------------------------------------------------
//  <copyright file = "UpsertOne.cs" company = "Prism">
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

public interface IUpsertOneService
{
    Task<string> Upsert<T>(T item)
        where T : IDataModel;
}

public class UpsertOneService : IUpsertOneService
{
    private readonly ILogger<UpsertOneService> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly User _user;

    public UpsertOneService(ILogger<UpsertOneService> logger, IOrganizationContext organizationContext, User user)
    {
        _organizationContext = organizationContext;
        _user = user;
        _logger = logger;
    }

    public async Task<string> Upsert<T>(T item)
        where T : IDataModel
    {
        var collection = _organizationContext.GetCollection<T>();

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            return await _logger.LogDataInsert(_user, item, async () =>
            {
                await collection.InsertOneAsync(item);
                return item.Id;
            });
        }

        return await _logger.LogDataUpdate(_user, item, async () =>
        {
            var updated = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", item.Id), item, new FindOneAndReplaceOptions<T>
            {
                IsUpsert = true
            });

            return updated.Id;
        });
    }
}