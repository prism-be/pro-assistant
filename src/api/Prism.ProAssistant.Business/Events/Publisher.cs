// -----------------------------------------------------------------------
//  <copyright file = "Publisher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Business.Events;

public interface IPublisher
{
    void Publish<T>(string exchange, T message);
}

public class Publisher : IPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<Publisher> _logger;

    public Publisher(IConnection connection, ILogger<Publisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public void Publish<T>(string exchange, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var payload = Encoding.Default.GetBytes(json);
        var channel = _connection.CreateModel();
        channel.BasicPublish(exchange, typeof(T).FullName, null, payload);
        _logger.LogInformation("Published message to exchange {exchange} with routing key {routingKey}", exchange, typeof(T).FullName);
    }
}