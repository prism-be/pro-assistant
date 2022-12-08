// -----------------------------------------------------------------------
//  <copyright file = "BaseUpdatedServiceBusWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public abstract class BaseUpdatedServiceBusWorker<TModel, TAction> : BaseServiceBusWorker<UpsertedItem<TModel>>
    where TAction : UpsertedItem<TModel>
{
    protected BaseUpdatedServiceBusWorker(ILogger<BaseUpdatedServiceBusWorker<TModel, TAction>> logger, IServiceProvider serviceProvider, IConnection? connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => Topics.GetExchangeName<TModel>(Topics.Actions.Updated);
    public override string WorkerName => typeof(TAction).FullName ?? string.Empty;

    public override async Task ProcessMessageAsync(IMediator mediator, UpsertedItem<TModel> payload)
    {
        var action = Activator.CreateInstance(typeof(TAction), payload.Previous, payload.Current);
        await mediator.Send(action!);
    }
}

public class UpdatedTariffServiceBusWorker : BaseUpdatedServiceBusWorker<Tariff, UpdateMeetingsColor>
{
    public UpdatedTariffServiceBusWorker(ILogger<UpdatedTariffServiceBusWorker> logger, IServiceProvider serviceProvider, IConnection? connection)
        : base(logger, serviceProvider, connection)
    {
    }
}