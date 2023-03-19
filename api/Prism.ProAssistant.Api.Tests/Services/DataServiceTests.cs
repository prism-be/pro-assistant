using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class DataServiceTests
{
    [Fact]
    public async Task DeleteAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var id = Identifier.GenerateString();

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.DeleteAsync<Contact>(id);

        // Assert
        collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task DeleteFileAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var id = Identifier.GenerateString();

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.DeleteFileAsync(id);

        // Assert
        bucket.Verify(x => x.DeleteAsync(new ObjectId(id), CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task GetFileAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.Setup(x => x.DownloadAsBytesAsync(new ObjectId(id), null, CancellationToken.None)).ReturnsAsync(new byte[] { 1, 2, 3 });
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var data = new byte[] { 1, 2, 3 };

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.GetFileAsync(id);

        // Assert
        result.Should().BeEquivalentTo(data);

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