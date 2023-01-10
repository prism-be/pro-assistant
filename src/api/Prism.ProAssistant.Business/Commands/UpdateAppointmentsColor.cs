// -----------------------------------------------------------------------
//  <copyright file = "UpdateAppointmentsColor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpdateAppointmentsColor(string TariffId, string Organization) : IRequest;

public class UpdateAppointmentsColorHandler : IRequestHandler<UpdateAppointmentsColor>
{
    private readonly ILogger<UpdateAppointmentsColorHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public UpdateAppointmentsColorHandler(ILogger<UpdateAppointmentsColorHandler> logger, IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateAppointmentsColor request, CancellationToken cancellationToken)
    {
        _organizationContext.SelectOrganization(request.Organization);

        var tariff = (await _organizationContext.GetCollection<Tariff>()
                .FindAsync(x => x.Id == request.TariffId, cancellationToken: cancellationToken))
            .FirstOrDefault();

        if (tariff == null)
        {
            _logger.LogWarning("Cannot update the appointments color. The tariff with id {itemId} does not exist.", request.TariffId);
            return Unit.Value;
        }

        var collection = _organizationContext.GetCollection<Appointment>();
        var appointments = await collection.FindAsync<Appointment>(Builders<Appointment>.Filter.Eq(nameof(Appointment.TypeId), request.TariffId), cancellationToken: cancellationToken);

        foreach (var appointment in appointments.ToList(cancellationToken: cancellationToken))
        {
            appointment.ForeColor = tariff.ForeColor;
            appointment.BackgroundColor = tariff.BackgroundColor;
            await collection.ReplaceOneAsync(Builders<Appointment>.Filter.Eq(nameof(Appointment.Id), appointment.Id), appointment, cancellationToken: cancellationToken);
            _logger.LogInformation("Replaced the appointment with id {itemId} with the new color {color}.", appointment.Id, appointment.BackgroundColor);
        }

        return Unit.Value;
    }
}