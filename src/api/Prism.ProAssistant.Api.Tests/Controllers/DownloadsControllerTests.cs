// -----------------------------------------------------------------------
//  <copyright file = "DownloadsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
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
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new DownloadController(mediator.Object, cache.Object);
        await controller.Delete(new DeleteDocument(documentId, appointmentId));

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<DeleteDocument>(), CancellationToken.None), Times.Once);
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

        // Act
        var controller = new DownloadController(Mock.Of<IMediator>(), cache.Object);
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
        var receiptGenerator = new Mock<IMediator>();
        receiptGenerator.Setup(x => x.Send(It.IsAny<GenerateDocument>(), CancellationToken.None))
            .ReturnsAsync(Guid.NewGuid().ToByteArray);

        // Act
        var controller = new DownloadController(receiptGenerator.Object, cache.Object);
        await controller.Generate(new GenerateDocument(documentId, appointmentId));

        // Assert
        receiptGenerator.Verify(x => x.Send(It.IsAny<GenerateDocument>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Receipt_NotFound()
    {
        // Arrange
        var appointmentKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((byte[])null!);

        // Act
        var controller = new DownloadController(Mock.Of<IMediator>(), cache.Object);
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
        var receiptGenerator = new Mock<IMediator>();
        receiptGenerator.Setup(x => x.Send(It.IsAny<DownloadDocument>(), CancellationToken.None))
            .ReturnsAsync(new DownloadDocumentResponse(documentId, Encoding.Default.GetBytes(documentId)));

        // Act
        var controller = new DownloadController(receiptGenerator.Object, cache.Object);
        var result = await controller.Start(new DownloadDocument(documentId));

        // Assert
        receiptGenerator.Verify(x => x.Send(It.IsAny<DownloadDocument>(), CancellationToken.None), Times.Once);
        result.Should().BeAssignableTo<ActionResult<DownloadKey>>();
    }

    [Fact]
    public async Task Start_NotFound()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var receiptGenerator = new Mock<IMediator>();

        // Act
        var controller = new DownloadController(receiptGenerator.Object, cache.Object);
        var result = await controller.Start(new DownloadDocument(documentId));

        // Assert
        receiptGenerator.Verify(x => x.Send(It.IsAny<DownloadDocument>(), CancellationToken.None), Times.Once);
        result.Should().BeAssignableTo<ActionResult<DownloadKey>>();
        result.Result.Should().BeAssignableTo<NotFoundResult>();
    }
}