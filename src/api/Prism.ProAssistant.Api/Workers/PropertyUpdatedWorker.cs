// -----------------------------------------------------------------------
//  <copyright file = "PropertyUpdatedWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class PropertyUpdatedWorker : BaseServiceBusWorker<PropertyUpdated>
{
    public PropertyUpdatedWorker(ILogger<PropertyUpdatedWorker> logger, IServiceProvider serviceProvider, IConnection? connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => "PropertyUpdated";
    public override string WorkerName => nameof(PropertyUpdatedWorker);

    public override async Task ProcessMessageAsync(IServiceProvider provider, Event<PropertyUpdated> e)
    {
        var payload = e.Payload;

        switch (payload.ItemType)
        {
            case nameof(Contact):
                switch (payload.Property)
                {
                    case nameof(Contact.BirthDate):
                        var user = provider.GetService<User>() ?? new User();
                        user.Id = e.User.Id;
                        user.Organization = e.User.Name;

                        var updateManyPropertyService = provider.GetService<UpdateManyPropertyService>() ?? throw new NotSupportedException();
                        await updateManyPropertyService.Update<Appointment>(nameof(Appointment.ContactId), payload.Id, nameof(Appointment.BirthDate), payload.Value);
                        break;
                }

                break;
        }
    }
}