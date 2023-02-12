// -----------------------------------------------------------------------
//  <copyright file = "TariffBackgroundColorUpdatedWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class TariffBackgroundColorUpdatedWorker : BaseServiceBusWorker<PropertyUpdated>
{
    
    public TariffBackgroundColorUpdatedWorker(ILogger<TariffBackgroundColorUpdatedWorker> logger, IServiceProvider serviceProvider, IConnection? connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => $"Property.Updated.{nameof(Tariff)}.{nameof(Tariff.BackgroundColor)}";
    public override string WorkerName => nameof(TariffBackgroundColorUpdatedWorker);

    public override async Task ProcessMessageAsync(IServiceProvider provider, Event<PropertyUpdated> e)
    {
        var crudService = provider.GetService<ICrudService>() ?? throw new NotSupportedException();
        await crudService.UpdateManyProperty<Appointment>(nameof(Appointment.TypeId), e.Payload.Id, nameof(Appointment.BackgroundColor), e.Payload.Value);
    }
}