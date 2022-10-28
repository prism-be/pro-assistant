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

    [UseSorting]
    public IExecutable<Meeting> GetMeetings(DateTime startDate, DateTime endDate, [Service] IOrganizationContext organizationContext)
    {
        var filters = new List<FilterDefinition<Meeting>>
        {
            Builders<Meeting>.Filter.Gte(x => x.StartDate, startDate),
            Builders<Meeting>.Filter.Lte(x => x.StartDate, endDate)
        };

        return organizationContext.Meetings
            .Find(Builders<Meeting>.Filter.And(filters))
            .AsExecutable();
    }
}