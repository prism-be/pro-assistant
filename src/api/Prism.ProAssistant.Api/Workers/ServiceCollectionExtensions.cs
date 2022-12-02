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

        services.AddScoped<IPublisher, Publisher>();

        services.AddHostedService<UpdateMeetingColorWorker>();
    }
}