// -----------------------------------------------------------------------
//  <copyright file = "MeetingMutation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Settings;

[Authorize]
[ExtendObjectType("Mutation")]
public class SettingMutation
{
    public async Task<Setting> UpsertSettingAsync(Setting setting, [Service] IOrganizationContext organizationContext, [Service] ILogger<SettingMutation> logger,
        [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("Updating settings {meetingId}", setting.Id);
        await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, setting));
        await organizationContext.Settings.ReplaceOneAsync(Builders<Setting>.Filter.Eq("Id", setting.Id), setting);
        return setting;
    }
}