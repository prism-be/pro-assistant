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

namespace Prism.ProAssistant.Api.Graph.Meetings;

[Authorize]
[ExtendObjectType("Query")]
public class MeetingQuery
{
    [UseFirstOrDefault]
    public IExecutable<Meeting> GetMeetingById(string id, [Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Meetings.Find(x => x.Id == id).AsExecutable();
    }

    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<Meeting> GetMeetings([Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Meetings.AsExecutable();
    }
}