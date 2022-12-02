// -----------------------------------------------------------------------
//  <copyright file = "UpdateMeetingsColor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpdateMeetingsColor(string TariffId, string Organization) : IRequest;

public class UpdateMeetingsColorHandler : IRequestHandler<UpdateMeetingsColor>
{
    private readonly ILogger<UpdateMeetingsColorHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public UpdateMeetingsColorHandler(IOrganizationContext organizationContext, ILogger<UpdateMeetingsColorHandler> logger)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateMeetingsColor request, CancellationToken cancellationToken)
    {
        _organizationContext.SelectOrganization(request.Organization);

        var tariff = (await _organizationContext.GetCollection<Tariff>()
                .FindAsync(x => x.Id == request.TariffId, cancellationToken: cancellationToken))
            .FirstOrDefault();

        if (tariff == null)
        {
            _logger.LogWarning("Cannot update the meetings color. The tariff with id {TariffId} does not exist.", request.TariffId);
            return Unit.Value;
        }

        var collection = _organizationContext.GetCollection<Meeting>();
        var meetings = await collection.FindAsync<Meeting>(Builders<Meeting>.Filter.Eq(nameof(Meeting.TypeId), request.TariffId), cancellationToken: cancellationToken);

        foreach (var meeting in meetings.ToList(cancellationToken: cancellationToken))
        {
            meeting.ForeColor = tariff.ForeColor;
            meeting.BackgroundColor = tariff.BackgroundColor;
            await collection.ReplaceOneAsync(Builders<Meeting>.Filter.Eq(nameof(Meeting.Id), meeting.Id), meeting, cancellationToken: cancellationToken);
            _logger.LogInformation("Replaced the meeting with id {MeetingId} with the new color {color}.", meeting.Id, meeting.BackgroundColor);
        }

        return Unit.Value;
    }
}