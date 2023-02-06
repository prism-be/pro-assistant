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
    void Publish<T>(string organizationId, string userId, string exchange, T message);
}

public record PropertyUpdated(string ItemType, string Id, string Property, object Value);

public record Event<T>(string OrganizationId, string UserId, T Payload);

public class Publisher : IPublisher
{
    private readonly IModel _channel;

    public Publisher(IModel channel)
    {
        _channel = channel;
    }

    public void Publish<T>(string organizationId, string userId, string exchange, T message)
    {
        var e = new Event<T>(organizationId, userId, message);
        var json = JsonSerializer.Serialize(e);
        var payload = Encoding.Default.GetBytes(json);
        _channel.BasicPublish(exchange, typeof(T).FullName, null, payload);
    }
}