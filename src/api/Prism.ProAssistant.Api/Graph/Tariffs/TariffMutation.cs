﻿// -----------------------------------------------------------------------
//  <copyright file = "TariffMutation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Tariffs;

[Authorize]
[ExtendObjectType("Mutation")]
public class TariffMutation
{
    public async Task<bool> RemoveTariffAsync(string id, [Service] IOrganizationContext organizationContext, [Service] ILogger<TariffMutation> logger)
    {
        logger.LogInformation("Removing tariffs {tarrifId}", id);
        var result = await organizationContext.Tariffs.DeleteOneAsync(Builders<Tariff>.Filter.Eq("Id", id));

        return result.IsAcknowledged;
    }

    public async Task<Tariff> UpsertTariffAsync(Tariff tariff, [Service] IOrganizationContext organizationContext, [Service] ILogger<TariffMutation> logger, [Service]IUserContextAccessor userContextAccessor)
    {
        if (string.IsNullOrWhiteSpace(tariff.Id))
        {
            await organizationContext.Tariffs.InsertOneAsync(tariff);
            await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, tariff));
            logger.LogInformation("Created new tariffs {tarrifId}", tariff.Id);
            return tariff;
        }
        
        logger.LogInformation("Updating tariffs {tarrifId}", tariff.Id);
        await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, tariff));
        return await organizationContext.Tariffs.FindOneAndReplaceAsync(Builders<Tariff>.Filter.Eq("Id", tariff.Id), tariff);
    }
}