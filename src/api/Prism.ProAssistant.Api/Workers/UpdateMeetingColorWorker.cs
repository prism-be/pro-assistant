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

    protected override string Queue => Topics.Tariffs.Updated;
    protected override string WorkerName => nameof(UpdateMeetingColorWorker);

    protected override async Task ProcessMessageAsync(IMediator mediator, UpsertResult payload)
    {
        await mediator.Send(new UpdateMeetingsColor(payload.Id, payload.Organization));
    }
}