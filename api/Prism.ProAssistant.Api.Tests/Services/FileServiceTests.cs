using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class FileServiceTests
{
    [Fact]
    public async Task DeleteFileAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<FileService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var id = Identifier.GenerateString();

        // Act
        var service = new FileService(logger.Object, userOrganizationService.Object);
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

        var logger = new Mock<ILogger<FileService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.Setup(x => x.DownloadAsBytesAsync(new ObjectId(id), null, CancellationToken.None)).ReturnsAsync(new byte[] { 1, 2, 3 });
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var data = new byte[] { 1, 2, 3 };

        // Act
        var service = new FileService(logger.Object, userOrganizationService.Object);
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

    [Fact]
    public async Task GetFileNameAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var fileInfo = new GridFSFileInfo(new BsonDocument
        {
            { "filename", id }
        });

        var logger = new Mock<ILogger<FileService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.SetupBucket(fileInfo);
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        // Act
        var service = new FileService(logger.Object, userOrganizationService.Object);
        var result = await service.GetFileNameAsync(id);

        // Assert
        result.Should().BeEquivalentTo(id);

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
    public async Task UploadFromBytesAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<FileService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.Setup(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), null, CancellationToken.None))
            .ReturnsAsync(new ObjectId(id));
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var data = new byte[] { 1, 2, 3 };

        // Act
        var service = new FileService(logger.Object, userOrganizationService.Object);
        var result = await service.UploadFromBytesAsync(Identifier.GenerateString(), data);

        // Assert
        result.Should().BeEquivalentTo(id);

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