// -----------------------------------------------------------------------
//  <copyright file = "PropertyUpdatedWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class PropertyUpdatedWorker : BaseServiceBusWorker<PropertyUpdated>
{
    private readonly IUpdateManyPropertyService _upddater;

    public PropertyUpdatedWorker(ILogger<PropertyUpdatedWorker> logger, IServiceProvider serviceProvider, IConnection? connection, IUpdateManyPropertyService upddater)
        : base(logger, serviceProvider, connection)
    {
        _upddater = upddater;
    }

    public override string Queue => Topics.PropertyUpdated;
    public override string WorkerName => nameof(PropertyUpdatedWorker);

    public override async Task ProcessMessageAsync(IMediator mediator, Event<PropertyUpdated> e)
    {
        var payload = e.Payload;
        switch (payload.ItemType)
        {
            case nameof(Contact):
                switch (payload.Property)
                {
                    case nameof(Contact.BirthDate):
                        await _upddater.UpdateMany<Appointment>(new UpdateManyProperty(e.OrganizationId, e.UserId, nameof(Appointment.ContactId), payload.Id, nameof(Appointment.BirthDate), payload.Value));
                        break;
                }

                break;
        }
    }
}