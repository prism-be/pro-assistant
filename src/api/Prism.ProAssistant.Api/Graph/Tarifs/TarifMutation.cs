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

namespace Prism.ProAssistant.Api.Graph.Tarifs;

[Authorize]
[ExtendObjectType("Mutation")]
public class TarifMutation
{
    public async Task<Tarif> CreateTarifAsync(Tarif tarif, [Service] IOrganizationContext organizationContext)
    {
        tarif.Id = Identifier.Generate();
        await organizationContext.Tarifs.InsertOneAsync(tarif);

        return tarif;
    }

    public async Task<bool> RemoveTarifAsync(Guid id, [Service] IOrganizationContext organizationContext)
    {
        var result = await organizationContext.Tarifs.DeleteOneAsync(Builders<Tarif>.Filter.Eq("Id", id));

        return result.IsAcknowledged;
    }

    public async Task<Tarif> UpdateTarifAsync(Tarif tarif, [Service] IOrganizationContext organizationContext)
    {
        return await organizationContext.Tarifs.FindOneAndReplaceAsync(Builders<Tarif>.Filter.Eq("Id", tarif.Id), tarif);
    }
}