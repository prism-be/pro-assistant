// -----------------------------------------------------------------------
//  <copyright file = "PatientQuery.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Tariffs;

[Authorize]
[ExtendObjectType("Query")]
public class TariffQuery
{
    [UseFirstOrDefault]
    public IExecutable<Tariff> GetTariffById(Guid id, [Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Tariffs.Find(x => x.Id == id).AsExecutable();
    }

    [UseSorting]
    [UseFiltering]
    public IExecutable<Tariff> GetTariffs([Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Tariffs.AsExecutable();
    }
}