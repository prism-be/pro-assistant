using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Prism.Core;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DocumentControllerTests
{
    [Fact]
    public async Task Download_NotFound()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        cache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null!);

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization.Object);
        var result = await controller.Download(Identifier.GenerateString(), Identifier.GenerateString(), true);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Download_Unknown()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        var documentId = Identifier.GenerateString();
        var keyId = Identifier.GenerateString();

        var fileReference = new DocumentController.FileReference(Identifier.GenerateString(), "unit test");

        cache.Setup(x => x.GetAsync(keyId, It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.Default.GetBytes(JsonSerializer.Serialize(fileReference)));

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization.Object);
        var result = await controller.Download(documentId, keyId, true);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Download_Ok()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        var documentId = Identifier.GenerateString();
        var keyId = Identifier.GenerateString();

        var data = RandomNumberGenerator.GetBytes(32);

        var fileReference = new DocumentController.FileReference(documentId, "unit test");

        cache.Setup(x => x.GetAsync(keyId, It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.Default.GetBytes(JsonSerializer.Serialize(fileReference)));
        dataStorage.Setup(x => x.OpenFileStreamAsync("unit test", "documents", documentId)).ReturnsAsync(new MemoryStream(data));

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization.Object);
        var result = await controller.Download(documentId, keyId, true);

        // Assert
        result.Should().BeOfType<FileStreamResult>();
    }

    [Fact]
    public async Task DownloadDocument_NotExists()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        var documentId = Identifier.GenerateString();
        dataStorage.Setup(x => x.ExistsAsync("unit test", "documents", documentId)).ReturnsAsync(false);

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization.Object);
        var act = async () => await controller.DownloadDocument(documentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DownloadDocument_Ok()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new UserOrganization
        {
            Organization = "unit test"
        };

        var documentId = Identifier.GenerateString();

        dataStorage.Setup(x => x.ExistsAsync("unit test", "documents", documentId)).ReturnsAsync(true);

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization);
        var result = await controller.DownloadDocument(documentId);

        // Assert
        result.Id.Should().NotBeNull();
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateDocument_Ok()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object, userOrganization.Object);
        await controller.GenerateDocument(new DocumentRequest
        {
            AppointmentId = Identifier.GenerateString(),
            DocumentId = Identifier.GenerateString()
        });

        // Assert
        pdfService.Verify(x => x.GenerateDocument(It.IsAny<DocumentRequest>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Ok()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var eventStore = new Mock<IEventStore>();
        var pdfService = new Mock<IPdfService>();
        var dataStorage = new Mock<IDataStorage>();
        var userOrganization = new Mock<UserOrganization>();

        // Act
        var controller = new DocumentController(cache.Object, eventStore.Object, pdfService.Object, dataStorage.Object,
            userOrganization.Object);
        await controller.Delete(Identifier.GenerateString(), Identifier.GenerateString());

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Appointment>(It.IsAny<DetachAppointmentDocument>()), Times.Once);
    }
}