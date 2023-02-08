// -----------------------------------------------------------------------
//  <copyright file = "Publisher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Prism.ProAssistant.Business.Security;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Business.Events;

public interface IPublisher
{
    void Publish<T>(string exchange, T message);
}

public record PropertyUpdated(string ItemType, string Id, string Property, object Value);

public record Event<T>(IUser User, T Payload);

public class Publisher : IPublisher
{
    private readonly IModel _channel;
    private readonly IUser _user;

    public Publisher(IModel channel, IUser user)
    {
        _channel = channel;
        _user = user;
    }

    public void Publish<T>(string exchange, T message)
    {
        var e = new Event<T>(_user, message);
        var json = JsonSerializer.Serialize(e);
        var payload = Encoding.Default.GetBytes(json);
        _channel.BasicPublish(exchange, typeof(T).FullName, null, payload);
    }
}