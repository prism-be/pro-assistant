using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DocumentControllerTests
{

    [Fact]
    public async Task Delete_Ok()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var appointmentId = Identifier.GenerateString();

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();

        var appointment = new Appointment
        {
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Id = Identifier.GenerateString(),
            Documents = new List<BinaryDocument>

            {
                new()
                {
                    Id = documentId,
                    FileName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                }
            }
        };

        dataService.Setup(x => x.SingleAsync<Appointment>(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        await controller.Delete(appointmentId, documentId);

        // Assert
        dataService.Verify(x => x.DeleteFileAsync(documentId), Times.Once);
        appointment.Documents.Should().BeEmpty();
    }

    [Fact]
    public async Task Download_NotFound()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var downloadKey = Identifier.GenerateString();

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();

        cache.Setup(x => x.GetAsync(downloadKey, CancellationToken.None)).ReturnsAsync(null as byte[]);

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        var result = await controller.Download(documentId, downloadKey, true);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Download_Ok()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var downloadKey = Identifier.GenerateString();

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();

        cache.Setup(x => x.GetAsync(downloadKey, CancellationToken.None)).ReturnsAsync(Array.Empty<byte>());

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        var result = await controller.Download(documentId, downloadKey, true);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<FileContentResult>();
    }

    [Fact]
    public async Task DownloadDocument_Ok()
    {
        // Arrange
        var documentId = Identifier.GenerateString();

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();

        dataService.Setup(x => x.GetFileAsync(documentId)).ReturnsAsync(Array.Empty<byte>());

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        var result = await controller.DownloadDocument(documentId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();

        cache.Verify(x => x.SetAsync(result.Id, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task DownloadDocument_NotFuund()
    {
        // Arrange
        var documentId = Identifier.GenerateString();

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();
        
        dataService.Setup(x => x.GetFileAsync(documentId)).ReturnsAsync(null as byte[]);

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        await controller.Invoking(c => c.DownloadDocument(documentId)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateDocument_Ok()
    {
        // Arrange
        var request = new DocumentRequest
        {
            AppointmentId = Identifier.GenerateString(),
            DocumentId = Identifier.GenerateString()
        };

        var cache = new Mock<IDistributedCache>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();
        var pdfService = new Mock<IPdfService>();

        // Act
        var controller = new DocumentController(pdfService.Object, dataService.Object, cache.Object, eventService.Object);
        await controller.GenerateDocument(request);

        // Assert
        pdfService.Verify(x => x.GenerateDocument(request), Times.Once);
    }
}