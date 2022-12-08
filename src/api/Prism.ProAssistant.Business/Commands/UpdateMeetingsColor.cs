// -----------------------------------------------------------------------
//  <copyright file = "UpdateMeetingsColor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpdateMeetingsColor(Tariff? Previous, Tariff Current, string Organization) : UpsertedItem<Tariff>(Previous, Current, Organization), IRequest;

public class UpdateMeetingsColorHandler : IRequestHandler<UpdateMeetingsColor>
{
    private readonly ILogger<UpdateMeetingsColorHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public UpdateMeetingsColorHandler(ILogger<UpdateMeetingsColorHandler> logger, IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateMeetingsColor request, CancellationToken cancellationToken)
    {
        if (request.Previous?.ForeColor == request.Current.ForeColor && request.Previous?.BackgroundColor == request.Current.BackgroundColor)
        {
            _logger.LogInformation("The color of the tariff {tariffId} has not changed", request.Current.Id);
            return Unit.Value;
        }

        _organizationContext.SelectOrganization(request.Organization);

        var collection = _organizationContext.GetCollection<Meeting>();
        var meetings = await collection.FindAsync<Meeting>(Builders<Meeting>.Filter.Eq(nameof(Meeting.TypeId), request.Current.Id), cancellationToken: cancellationToken);

        foreach (var meeting in meetings.ToList(cancellationToken: cancellationToken))
        {
            
            if (meeting.BackgroundColor == request.Current.BackgroundColor && meeting.ForeColor == request.Current.ForeColor)
            {
                _logger.LogInformation("Meeting {meeting} has the same color as the tariff {tariffId}", meeting.Id, request.Current.Id);
                continue;
            }
            
            var update = Builders<Meeting>.Update.Combine(
                Builders<Meeting>.Update.Set(nameof(Meeting.ForeColor), request.Current.ForeColor),
                Builders<Meeting>.Update.Set(nameof(Meeting.BackgroundColor), request.Current.BackgroundColor)
            );

            await collection.UpdateOneAsync(Builders<Meeting>.Filter.Eq(nameof(Meeting.Id), meeting.Id), update, cancellationToken: cancellationToken);

            _logger.LogInformation("Updated the meeting with id {meetingId} with the new color {color}.", meeting.Id, meeting.BackgroundColor);
        }

        return Unit.Value;
    }
}