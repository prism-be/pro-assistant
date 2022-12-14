// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using RabbitMQ.Client;
using IPublisher = Prism.ProAssistant.Business.Events.IPublisher;

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
        DeclareExchanges(channel);

        services.AddScoped<IPublisher, Publisher>();

        services.AddServiceBusWorker<Tariff>(nameof(UpdateMeetingsColor), payload => new UpdateMeetingsColor(payload.Previous, payload.Current, payload.Organisation));
    }

    private static void AddServiceBusWorker<T>(this IServiceCollection services, string workerName, Func<UpsertedItem<T>, IRequest> requestFactory)
    {
        services.AddHostedService<DataUpdatedServiceBusWorker<T>>(provider =>
        {
            var logger = provider.GetService<ILogger<DataUpdatedServiceBusWorker<T>>>();

            if (logger == null)
            {
                throw new NotSupportedException("Logger is not registered");
            }
            
            var connexion = provider.GetService<IConnection>();
            var queue = Topics.GetExchangeName<T>(Topics.Actions.Updated);

            return new DataUpdatedServiceBusWorker<T>(logger, provider, connexion, requestFactory, queue, workerName);
        });
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