// -----------------------------------------------------------------------
//  <copyright file = "UpdateAppointmentColorWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class UpdateAppointmentColorWorker : BaseServiceBusWorker<UpsertResult>
{
    public UpdateAppointmentColorWorker(ILogger<UpdateAppointmentColorWorker> logger, IServiceProvider serviceProvider, IConnection connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => Topics.Tariffs.Updated;
    public override string WorkerName => nameof(UpdateAppointmentColorWorker);

    public override async Task ProcessMessageAsync(IMediator mediator, Event<UpsertResult> e)
    {
        await mediator.Send(new UpdateAppointmentsColor(e.Payload.Id, e.OrganizationId));
    }
}