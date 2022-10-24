// -----------------------------------------------------------------------
//  <copyright file = "PatientMutation.cs" company = "Prism">
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
    public async Task<Tariff> CreateTariffAsync(Tariff tariff, [Service] IOrganizationContext organizationContext)
    {
        tariff.Id = Identifier.Generate();
        await organizationContext.Tariffs.InsertOneAsync(tariff);

        return tariff;
    }

    public async Task<bool> RemoveTariffAsync(Guid id, [Service] IOrganizationContext organizationContext)
    {
        var result = await organizationContext.Tariffs.DeleteOneAsync(Builders<Tariff>.Filter.Eq("Id", id));

        return result.IsAcknowledged;
    }

    public async Task<Tariff> UpdateTariffAsync(Tariff tariff, [Service] IOrganizationContext organizationContext)
    {
        return await organizationContext.Tariffs.FindOneAndReplaceAsync(Builders<Tariff>.Filter.Eq("Id", tariff.Id), tariff);
    }
}