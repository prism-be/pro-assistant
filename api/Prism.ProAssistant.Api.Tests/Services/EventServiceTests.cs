using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class EventServiceTests
{
    [Fact]
    public async Task CreateAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();

        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);
        
        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        await service.CreateAsync(contact);

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(id), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }
}