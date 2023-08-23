namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Accounting.Forecast;

public class AccountingForecastEventServiceBusListener : BaseDomainEventServiceBusListener<Forecast>
{
    public AccountingForecastEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}