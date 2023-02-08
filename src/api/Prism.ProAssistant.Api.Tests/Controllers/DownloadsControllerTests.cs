// -----------------------------------------------------------------------
//  <copyright file = "DownloadsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DownloadsControllerTests
{
    [Fact]
    public async Task Delete()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var appointmentId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);
        await controller.Delete(new DeleteDocument(documentId, appointmentId));

        // Assert
        deleteDocumentService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Download_Ok(bool download)
    {
        // Arrange
        var appointmentId = Identifier.GenerateString();
        var appointmentKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(Encoding.Default.GetBytes(JsonSerializer.Serialize(new DownloadDocumentResponse(appointmentId, Encoding.Default.GetBytes(appointmentId)))));
        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);
        var result = await controller.Download(appointmentKey, download);

        // Assert
        result.Should().BeAssignableTo<FileContentResult>();
    }

    [Fact]
    public async Task Generate()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var appointmentId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();
        generateDocumentService.Setup(x => x.Generate(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Guid.NewGuid().ToByteArray);

        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);
        await controller.Generate(new GenerateDocument(documentId, appointmentId));

        // Assert
        generateDocumentService.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Receipt_NotFound()
    {
        // Arrange
        var appointmentKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((byte[])null!);
        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);
        var result = await controller.Download(appointmentKey, false);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Start()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();

        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();
        downloadDocumentService.Setup(x => x.Download(It.IsAny<string>()))
            .ReturnsAsync(new DownloadDocumentResponse(documentId, Encoding.Default.GetBytes(documentId)));

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);
        var result = await controller.Start(new DownloadDocument(documentId));

        // Assert
        downloadDocumentService.Verify(x => x.Download(It.IsAny<string>()), Times.Once);
        result.Should().BeAssignableTo<ActionResult<DownloadKey>>();
    }

    [Fact]
    public async Task Start_NotFound()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var deleteDocumentService = new Mock<IDeleteDocumentService>();
        var downloadDocumentService = new Mock<IDownloadDocumentService>();
        var generateDocumentService = new Mock<IGenerateDocumentService>();
        downloadDocumentService.Setup(x => x.Download(It.IsAny<string>()))
            .ReturnsAsync(new DownloadDocumentResponse(documentId, Encoding.Default.GetBytes(documentId)));

        // Act
        var controller = new DownloadController(cache.Object, deleteDocumentService.Object, generateDocumentService.Object, downloadDocumentService.Object);

        var result = await controller.Start(new DownloadDocument(documentId));

        // Assert
        downloadDocumentService.Verify(x => x.Download(It.IsAny<string>()), Times.Once);
        result.Should().BeAssignableTo<ActionResult<DownloadKey>>();
        result.Result.Should().BeAssignableTo<NotFoundResult>();
    }
}