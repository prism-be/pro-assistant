// -----------------------------------------------------------------------
//  <copyright file = "TariffMutation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Tariffs;

[Authorize]
[ExtendObjectType("Mutation")]
public class TariffMutation
{
    public async Task<bool> RemoveTariffAsync(Guid id, [Service] IOrganizationContext organizationContext, [Service] ILogger<TariffMutation> logger)
    {
        logger.LogInformation("Removing tariffs {tarrifId}", id);
        var result = await organizationContext.Tariffs.DeleteOneAsync(Builders<Tariff>.Filter.Eq("Id", id));

        return result.IsAcknowledged;
    }

    public async Task<Tariff> UpsertTariffAsync(Tariff tariff, [Service] IOrganizationContext organizationContext, [Service] ILogger<TariffMutation> logger)
    {
        var options = new FindOneAndReplaceOptions<Tariff>
        {
            IsUpsert = true
        };
        
        logger.LogInformation("Upserting tariffs {tarrifId}", tariff.Id);
        return await organizationContext.Tariffs.FindOneAndReplaceAsync(Builders<Tariff>.Filter.Eq("Id", tariff.Id), tariff, options);
    }
}