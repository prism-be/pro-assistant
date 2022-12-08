// -----------------------------------------------------------------------
//  <copyright file = "PublisherTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Events;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Business.Tests.Events;

public class PublisherTests
{
    [Fact]
    public void Publish_Ok()
    {
        // Arrange
        var connection = new Mock<IConnection>();
        var channel = new Mock<IModel>();

        connection.Setup(x => x.CreateModel()).Returns(channel.Object);

        // Act
        var publisher = new Publisher(connection.Object, Mock.Of<ILogger<Publisher>>());
        publisher.Publish("test", "test");

        // Assert
        channel.Verify(x => x.BasicPublish("test", "System.String", false, null, It.IsAny<ReadOnlyMemory<Byte>>()), Times.Once);
    }
}