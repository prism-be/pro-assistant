namespace Prism.ProAssistant.Api.Services;

using System.Reflection;
using Azure.Messaging.ServiceBus;
using Core.Attributes;
using Infrastructure.Authentication;
using Storage;
using Storage.Events;

public class DomainEventServiceBusListener : BackgroundService
{
    private readonly Dictionary<string, Type> _effects = new();

    private readonly ServiceBusReceiver _receiver;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _receiver = serviceBusClient.CreateReceiver("domain/events");

        BuildEffectList();
    }

    public bool Running { get; set; } = true;

    private void BuildEffectList()
    {
        var types = typeof(QueryService).Assembly.GetTypes();

        foreach (var type in types)
        {
            var effectAttributes = type.GetCustomAttributes<SideEffectAttribute>();
            foreach (var effectAttribute in effectAttributes)
            {
                _effects.Add(effectAttribute.Key, type);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (Running)
        {
            var message = await _receiver.ReceiveMessageAsync(cancellationToken: stoppingToken);

            if (message == null)
            {
                continue;
            }

            var data = message.Body.ToObjectFromJson<EventContext>();

            await ProcessMessage(data);

            await _receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }

    public async Task ProcessMessage(EventContext data)
    {
        var key = $"{data.Event.StreamType}:{data.Event.Type}";

        if (_effects.TryGetValue(key, out var effectType))
        {
            using var scope = _serviceProvider.CreateScope();

            var userOrganization = scope.ServiceProvider.GetRequiredService<UserOrganization>();
            userOrganization.Id = data.Context.Id;
            userOrganization.Organization = data.Context.Organization;

            var effect = scope.ServiceProvider.GetRequiredService(effectType);
            var method = effectType.GetMethod("Handle");

            if (method == null)
            {
                throw new NotSupportedException($"Effect {effectType.Name} does not have a Handle method");
            }

            await (Task)method.Invoke(effect, new object[] { data.Event })!;
        }
    }
}