// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Events;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.Workers;

public static class ServiceCollectionExtensions
{
    public static void AddWorkers(this IServiceCollection services)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(EnvironmentConfiguration.GetMandatoryConfiguration("RABBITMQ_CONNECTION_STRING"))
        };

        var connection = factory.CreateConnection();
        services.AddSingleton(connection);

        var channel = connection.CreateModel();
        services.AddSingleton(channel);

        foreach (var watchedProperty in PropertyUpdatePublisher.WatchedProperties)
        {
            channel.ExchangeDeclare($"Property.Updated.{watchedProperty}", ExchangeType.Fanout);
        }

        services.AddScoped<IPublisher, Publisher>();
        services.AddScoped<IPropertyUpdatePublisher, PropertyUpdatePublisher>();

        services.AddHostedService<TariffBackgroundColorUpdatedWorker>();
        services.AddHostedService<ContactPhoneNumberUpdatedWorker>();
        services.AddHostedService<ContactBirthDateUpdatedWorker>();
    }
}