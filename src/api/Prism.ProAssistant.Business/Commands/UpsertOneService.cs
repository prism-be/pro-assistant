// -----------------------------------------------------------------------
//  <copyright file = "UpsertOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public interface IUpsertOneService
{
    Task<string> Upsert<T>(string organizationId, string userId, T item)
        where T : IDataModel;
}

public class UpsertOneService : IUpsertOneService
{
    private readonly ILogger<UpsertOneService> _logger;

    private readonly IOrganizationContext _organizationContext;

    public UpsertOneService(ILogger<UpsertOneService> logger, IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<string> Upsert<T>(string organizationId, string userId, T item)
        where T : IDataModel
    {
        _organizationContext.SelectOrganization(organizationId);
        var collection = _organizationContext.GetCollection<T>();

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            return await _logger.LogDataInsert(organizationId, userId, item, async () =>
            {
                await collection.InsertOneAsync(item);
                return item.Id;
            });
        }

        return await _logger.LogDataUpdate(organizationId, userId, item, async () =>
        {
            var updated = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", item.Id), item, new FindOneAndReplaceOptions<T>
            {
                IsUpsert = true
            });

            return updated.Id;
        });
    }
}