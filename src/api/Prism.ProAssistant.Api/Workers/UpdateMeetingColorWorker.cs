// -----------------------------------------------------------------------
//  <copyright file = "UpdateMeetingColorWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class UpdateMeetingColorWorker : BaseServiceBusWorker<UpsertResult>
{
    public UpdateMeetingColorWorker(ILogger<UpdateMeetingColorWorker> logger, IServiceProvider serviceProvider, IConnection connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => Topics.Tariffs.Updated;
    public override string WorkerName => nameof(UpdateMeetingColorWorker);

    public override async Task ProcessMessageAsync(IMediator mediator, UpsertResult payload)
    {
        await mediator.Send(new UpdateMeetingsColor(payload.Id, payload.Organization));
    }
}