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
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Download_Ok(bool download)
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var meetingKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(Encoding.Default.GetBytes(JsonSerializer.Serialize(new DownloadDocumentResponse(meetingId, Encoding.Default.GetBytes(meetingId)))));

        // Act
        var controller = new DownloadController(Mock.Of<IMediator>(), cache.Object);
        var result = await controller.Download(meetingKey, download);

        // Assert
        result.Should().BeAssignableTo<FileContentResult>();
    }

    [Fact]
    public async Task Generate()
    {
        // Arrange
        var documentId = Identifier.GenerateString();
        var meetingId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var receiptGenerator = new Mock<IMediator>();
        receiptGenerator.Setup(x => x.Send(It.IsAny<GenerateDocument>(), CancellationToken.None))
            .ReturnsAsync(Guid.NewGuid().ToByteArray);

        // Act
        var controller = new DownloadController(receiptGenerator.Object, cache.Object);
        await controller.Generate(new GenerateDocument(documentId, meetingId));

        // Assert
        receiptGenerator.Verify(x => x.Send(It.IsAny<GenerateDocument>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Receipt_NotFound()
    {
        // Arrange
        var meetingKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((byte[])null!);

        // Act
        var controller = new DownloadController(Mock.Of<IMediator>(), cache.Object);
        var result = await controller.Download(meetingKey, false);

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