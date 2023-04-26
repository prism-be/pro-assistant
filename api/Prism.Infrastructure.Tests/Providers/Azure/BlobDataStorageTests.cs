using System.Security.Cryptography;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers.Azure;

namespace Prism.Infrastructure.Tests.Providers.Azure;

public class BlobDataStorageTests
{
    [Fact]
    public async Task Test_All()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var data = RandomNumberGenerator.GetBytes(32);

        var blobDataStorage = new BlobDataStorageFake();
        var blobClient = new Mock<BlobClient>();
        blobDataStorage.Mock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClient.Object);

        var memoryStream = new MemoryStream();

        blobClient.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, null!));
        blobClient.Setup(x => x.OpenWriteAsync(true, It.IsAny<BlobOpenWriteOptions>(), CancellationToken.None))
            .ReturnsAsync(memoryStream);
        blobClient.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), CancellationToken.None))
            .ReturnsAsync(Response.FromValue(new BlobProperties
            {
                Metadata = { new KeyValuePair<string, string>("FileName", "fileName") }
            }, null!));
        blobClient.Setup(x => x.OpenReadAsync(0L, null, It.IsAny<BlobRequestConditions>(), CancellationToken.None))
            .ReturnsAsync(new MemoryStream(data));

        // Act
        var exists = await blobDataStorage.ExistsAsync("organization", "container", id);
        var stream = await blobDataStorage.CreateFileStreamAsync("organization", "container", "fileName", id);
        stream.Write(data);
        await stream.DisposeAsync();
        var fileName = await blobDataStorage.GetFileNameAsync("organization", "container", id);
        stream = await blobDataStorage.OpenFileStreamAsync("organization", "container", id);
        var readData = new byte[32];
        var readAsync = await stream.ReadAsync(readData).ConfigureAwait(false);
        await stream.DisposeAsync();
        await blobDataStorage.DeleteAsync("organization", "container", id);

        // Assert
        exists.Should().BeFalse();
        memoryStream.ToArray().Should().BeEquivalentTo(data);
        fileName.Should().Be("fileName");
        readAsync.Should().Be(32);
        readData.Should().BeEquivalentTo(data);
    }

    public class BlobDataStorageFake : BlobDataStorage
    {
        public Mock<BlobContainerClient> Mock { get; set; } = new();

        protected override BlobContainerClient GetBlobClient(string organization)
        {
            return Mock.Object;
        }
    }
}