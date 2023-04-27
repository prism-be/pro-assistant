namespace Prism.Infrastructure.Tests.Providers.Azure;

using Core;
using global::Azure.Messaging.ServiceBus;
using Infrastructure.Providers.Azure;
using Moq;

public class ServiceBusPublisherTests
{
    [Fact]
    public async Task PublishAsync_Ok()
    {
        // Arrange
        var serviceBusClient = new Mock<ServiceBusClient>();
        var queue = Identifier.GenerateString();
        
        var sender = new Mock<ServiceBusSender>();
        serviceBusClient.Setup(x => x.CreateSender(queue)).Returns(sender.Object);

        var message = new { Id = Identifier.GenerateString() };
        
        // Act
        var serviceBusPublisher = new ServiceBusPublisher(serviceBusClient.Object);
        await serviceBusPublisher.PublishAsync(queue, message);

        // Assert
        sender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}