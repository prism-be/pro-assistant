namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Configuration.Settings;

public class SettingsEventServiceBusListener: BaseDomainEventServiceBusListener<Setting>
{
    public SettingsEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}