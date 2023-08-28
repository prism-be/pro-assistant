namespace Prism.ProAssistant.Api.Services.Listeners;

using System.Reflection;
using Azure.Messaging.ServiceBus;
using Core.Attributes;
using Infrastructure.Authentication;
using Storage;
using Storage.Events;

public class BaseDomainEventServiceBusListener<T> : BackgroundService
{
    private readonly Dictionary<string, List<Type>> _effects = new();

    private readonly ServiceBusReceiver _receiver;
    private readonly IServiceProvider _serviceProvider;

    public BaseDomainEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var collectionName = CollectionAttribute.GetCollectionName<T>();

        _receiver = serviceBusClient.CreateReceiver("domain/events/" + collectionName);

        BuildEffectList();
    }

    public bool Running { get; set; } = true;

    private void BuildEffectList()
    {
        var types = typeof(QueryService).Assembly.GetTypes();

        foreach (var type in types)
        {
            var effectAttributes = type.GetCustomAttributes<SideEffectAttribute>();

            foreach (var effectAttributeKey in effectAttributes.Select(k => k.Key))
            {
                var effects = _effects.TryGetValue(effectAttributeKey, out var effect) ? effect : new List<Type>();
                effects.Add(type);
                _effects.Add(effectAttributeKey, effects);
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

            var data = message.Body.ToObjectFromJson<EventContext<T>>();

            await ProcessMessage(data);

            await _receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }

    public async Task ProcessMessage(EventContext<T> data)
    {
        var key = CollectionAttribute.GetCollectionName<T>();

        if (_effects.TryGetValue(key, out var effectTypes))
        {
            using var scope = _serviceProvider.CreateScope();

            var userOrganization = scope.ServiceProvider.GetRequiredService<UserOrganization>();
            userOrganization.Id = data.Context.Id;
            userOrganization.Organization = data.Context.Organization;

            var tasks = new List<Task>();
            foreach (var effectType in effectTypes)
            {
                var effect = scope.ServiceProvider.GetRequiredService(effectType);
                var method = effectType.GetMethod("Handle");

                if (method == null)
                {
                    throw new NotSupportedException($"Effect {effectType.Name} does not have a Handle method");
                }

                tasks.Add((Task)method.Invoke(effect, new object[] { data })!);
            }

            await Task.WhenAll(tasks);
        }
    }
}