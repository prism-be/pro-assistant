// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
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
        DeclareExchanges(channel);

        services.AddScoped<IPublisher, Publisher>();

        services.AddHostedService<UpdatedTariffServiceBusWorker>();
    }

    private static void DeclareExchanges(IModel channel)
    {
        channel.ExchangeDeclare(Topics.GetExchangeName<Document>(Topics.Actions.Updated), "fanout", true);
        channel.ExchangeDeclare(Topics.GetExchangeName<DocumentConfiguration>(Topics.Actions.Updated), "fanout", true);
        channel.ExchangeDeclare(Topics.GetExchangeName<Meeting>(Topics.Actions.Updated), "fanout", true);
        channel.ExchangeDeclare(Topics.GetExchangeName<Patient>(Topics.Actions.Updated), "fanout", true);
        channel.ExchangeDeclare(Topics.GetExchangeName<Setting>(Topics.Actions.Updated), "fanout", true);
        channel.ExchangeDeclare(Topics.GetExchangeName<Tariff>(Topics.Actions.Updated), "fanout", true);
    }
}