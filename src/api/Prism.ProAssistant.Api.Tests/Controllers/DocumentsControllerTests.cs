// -----------------------------------------------------------------------
//  <copyright file = "DocumentsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
        var receiptGenerator = new Mock<IReceiptGenerator>();
        receiptGenerator.Setup(x => x.Generate(meetingId)).ReturnsAsync((byte[])null!);

        // Act
        var controller = new DocumentsController(receiptGenerator.Object);
        var result = await controller.Receipt(meetingId);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Receipt_Ok()
    {
        // Arrange
        var meetingId = Identifier.GenerateString();
        var receiptGenerator = new Mock<IReceiptGenerator>();
        receiptGenerator.Setup(x => x.Generate(meetingId)).ReturnsAsync(Guid.NewGuid().ToByteArray);

        // Act
        var controller = new DocumentsController(receiptGenerator.Object);
        var result = await controller.Receipt(meetingId);

        // Assert
        result.Should().BeAssignableTo<FileStreamResult>();
    }
}