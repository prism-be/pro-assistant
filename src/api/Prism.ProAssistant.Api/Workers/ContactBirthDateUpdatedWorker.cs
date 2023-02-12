// -----------------------------------------------------------------------
//  <copyright file = "ContactPhoneNumberUpdatedWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public class ContactBirthDateUpdatedWorker : BaseServiceBusWorker<PropertyUpdated>
{
    public ContactBirthDateUpdatedWorker(ILogger<ContactBirthDateUpdatedWorker> logger, IServiceProvider serviceProvider, IConnection? connection)
        : base(logger, serviceProvider, connection)
    {
    }

    public override string Queue => $"Property.Updated.{nameof(Contact)}.{nameof(Contact.BirthDate)}";
    public override string WorkerName => nameof(ContactBirthDateUpdatedWorker);

    public override async Task ProcessMessageAsync(IServiceProvider provider, Event<PropertyUpdated> e)
    {
        var crudService = provider.GetService<ICrudService>() ?? throw new NotSupportedException();
        await crudService.UpdateManyProperty<Appointment>(nameof(Appointment.ContactId), e.Payload.Id, nameof(Appointment.BirthDate), e.Payload.Value);
    }
}