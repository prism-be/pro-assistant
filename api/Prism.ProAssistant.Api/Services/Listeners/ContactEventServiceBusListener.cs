namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.DayToDay.Contacts;

public class ContactEventServiceBusListener : BaseDomainEventServiceBusListener<Contact>
{
    public ContactEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider)
        : base(serviceBusClient, serviceProvider)
    {
    }
}