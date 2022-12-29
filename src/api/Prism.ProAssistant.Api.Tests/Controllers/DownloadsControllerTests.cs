// -----------------------------------------------------------------------
//  <copyright file = "DownloadsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task Download_Ok()
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var meetingKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(Encoding.Default.GetBytes(meetingId));

        // Act
        var controller = new DownloadController(Mock.Of<IMediator>(), cache.Object);
        var result = await controller.Download(meetingKey);

        // Assert
        result.Should().BeAssignableTo<FileStreamResult>();
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
        var result = await controller.Download(meetingKey);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Start()
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
}