namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Configuration.Tariffs;

public class TariffEventServiceBusListener: BaseDomainEventServiceBusListener<Tariff>
{
    public TariffEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}