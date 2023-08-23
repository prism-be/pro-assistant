namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.Accounting.Document;

public class AccountingDocumentEventServiceBusListener : BaseDomainEventServiceBusListener<AccountingDocument>
{
    public AccountingDocumentEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}