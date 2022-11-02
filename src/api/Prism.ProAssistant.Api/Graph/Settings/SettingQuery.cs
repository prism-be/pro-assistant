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

namespace Prism.ProAssistant.Api.Graph.Settings;

[Authorize]
[ExtendObjectType("Query")]
public class SettingQuery
{
    [UseFirstOrDefault]
    public IExecutable<Setting> GetSettingById(string id, [Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Settings.Find(x => x.Id == id).AsExecutable();
    }
}