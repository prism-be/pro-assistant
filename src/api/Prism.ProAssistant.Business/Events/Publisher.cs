// -----------------------------------------------------------------------
//  <copyright file = "Publisher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Business.Events;

public interface IPublisher
{
    void Publish<T>(string exchange, T message);
}

public class Publisher : IPublisher
{
    private readonly IModel _channel;

    public Publisher(IModel channel)
    {
        _channel = channel;
    }

    public void Publish<T>(string exchange, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var payload = Encoding.Default.GetBytes(json);
        _channel.BasicPublish(exchange, typeof(T).FullName, null, payload);
    }
}