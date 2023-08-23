namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Configuration.DocumentConfiguration;

public class DocumentConfigurationEventServiceBusListener: BaseDomainEventServiceBusListener<DocumentConfiguration>
{
    public DocumentConfigurationEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}