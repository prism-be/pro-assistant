// -----------------------------------------------------------------------
//  <copyright file = "PublisherTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Security;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Business.Tests.Events;

public class PublisherTests
{
    [Fact]
    public void Publish_Ok()
    {
        // Arrange
        var channel = new Mock<IModel>();
        var user = new Mock<User>();

        // Act
        var publisher = new Publisher(channel.Object, user.Object, Mock.Of<ILogger<Publisher>>());
        publisher.Publish("test", "test");

        // Assert
        channel.Verify(x => x.BasicPublish("test", "System.String", false, null, It.IsAny<ReadOnlyMemory<Byte>>()), Times.Once);
    }
}