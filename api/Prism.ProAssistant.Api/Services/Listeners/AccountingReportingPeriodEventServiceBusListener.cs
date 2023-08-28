namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Accounting.Reporting;

public class AccountingReportingPeriodEventServiceBusListener : BaseDomainEventServiceBusListener<AccountingReportingPeriod>
{
    public AccountingReportingPeriodEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}