// -----------------------------------------------------------------------
//  <copyright file = "BaseUpdatedServiceBusWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business.Events;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class DataUpdatedServiceBusWorker<TModel> : BaseServiceBusWorker<UpsertedItem<TModel>>
{

    private readonly Func<UpsertedItem<TModel>, IRequest> _factory;

    private readonly string _queue;
    private readonly string _workerName;

    public DataUpdatedServiceBusWorker(ILogger<DataUpdatedServiceBusWorker<TModel>> logger, IServiceProvider serviceProvider, IConnection? connection,
        Func<UpsertedItem<TModel>, IRequest> factory, string queue, string workerName)
        : base(logger, serviceProvider, connection)
    {
        _factory = factory;
        _queue = queue;
        _workerName = workerName;
    }

    public override string Queue => _queue;
    public override string WorkerName => _workerName;

    public override async Task ProcessMessageAsync(IMediator mediator, UpsertedItem<TModel> payload)
    {
        var action = _factory(payload);
        await mediator.Send(action);
    }
}