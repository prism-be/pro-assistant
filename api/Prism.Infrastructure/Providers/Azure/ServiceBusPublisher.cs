namespace Prism.Infrastructure.Providers.Azure;

using System.Text.Json;
using global::Azure.Messaging.ServiceBus;

public class ServiceBusPublisher : IPublisher
{
    private readonly ServiceBusClient _serviceBusClient;

    public ServiceBusPublisher(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public async Task PublishAsync<T>(string queue, T message)
    {
        var sender = _serviceBusClient.CreateSender(queue);
        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message);
        var serviceBusMessage = new ServiceBusMessage(messageBytes);
        await sender.SendMessageAsync(serviceBusMessage);
    }
}