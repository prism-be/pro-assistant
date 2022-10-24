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

namespace Prism.ProAssistant.Api.Graph.Tarifs;

[Authorize]
[ExtendObjectType("Query")]
public class TarifQuery
{
    [UseFirstOrDefault]
    public IExecutable<Tarif> GetTarifById(Guid id, [Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Tarifs.Find(x => x.Id == id).AsExecutable();
    }

    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<Tarif> GetTarifs([Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Tarifs.AsExecutable();
    }
}