// -----------------------------------------------------------------------
//  <copyright file = "DocumentsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Generators;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DocumentsControllerTests
{
    [Fact]
    public async Task Receipt_NotFound()
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var meetingKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((byte[]) null!);
        var receiptGenerator = new Mock<IReceiptGenerator>();

        // Act
        var controller = new DocumentsController(receiptGenerator.Object, cache.Object);
        var result = await controller.Receipt(meetingKey);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Receipt_Ok()
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var meetingKey = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(Encoding.Default.GetBytes(meetingId));
        var receiptGenerator = new Mock<IReceiptGenerator>();
        receiptGenerator.Setup(x => x.Generate(meetingId)).ReturnsAsync(Guid.NewGuid().ToByteArray);

        // Act
        var controller = new DocumentsController(receiptGenerator.Object, cache.Object);
        var result = await controller.Receipt(meetingKey);

        // Assert
        result.Should().BeAssignableTo<FileStreamResult>();
    }

    [Fact]
    public async Task Receipt_StartReceipt()
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var receiptGenerator = new Mock<IReceiptGenerator>();
        receiptGenerator.Setup(x => x.Generate(meetingId)).ReturnsAsync((byte[])null!);

        // Act
        var controller = new DocumentsController(receiptGenerator.Object, cache.Object);
        var result = await controller.StartReceipt(meetingId);

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }
}