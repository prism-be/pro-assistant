// -----------------------------------------------------------------------
//  <copyright file = "PatientMutations.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Graph.Patients;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Meetings;

[Authorize]
[ExtendObjectType("Mutation")]
public class MeetingMutation
{
    public async Task<Meeting> UpsertMeetingAsync(Meeting meeting, [Service] IOrganizationContext organizationContext, [Service] ILogger<MeetingMutation> logger, [Service]IUserContextAccessor userContextAccessor)
    {
        if (string.IsNullOrWhiteSpace(meeting.Id))
        {
            await organizationContext.Meetings.InsertOneAsync(meeting);
            await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, meeting));
            logger.LogInformation("Created new meeting {meetingId}", meeting.Id);
            return meeting;
        }
        
        logger.LogInformation("Updating meeting {meetingId}", meeting.Id);
        await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, meeting));
        return await organizationContext.Meetings.FindOneAndReplaceAsync(Builders<Meeting>.Filter.Eq("Id", meeting.Id), meeting);
    }
}