// -----------------------------------------------------------------------
//  <copyright file = "DeleteDocumentHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;
using Prism.ProAssistant.UnitTesting.Extensions;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests;

public class DeleteDocumentHandlerTests
{
    [Fact]
    public async Task Handle_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var meetingId = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        var bucket = organization.SetupBucket();
        organization.SetupCollection(new Meeting
        {
            Id = meetingId,
            Documents = new List<BinaryDocument>()
        });

        var logger = new Mock<ILogger<DeleteDocumentHandler>>();

        // Act
        var handler = new DeleteDocumentHandler(logger.Object, organization.Object);
        var result = await handler.Handle(new DeleteDocument(id, meetingId), default);

        // Assert
        result.Should().NotBeNull();
        bucket.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), CancellationToken.None), Times.Never);
        logger.VerifyLog(LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var meetingId = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        var bucket = organization.SetupBucket();
        organization.SetupCollection(new Meeting
        {
            Id = meetingId,
            Documents = new List<BinaryDocument>
            {
                new()
                {
                    Id = id
                }
            }
        });

        var logger = new Mock<ILogger<DeleteDocumentHandler>>();

        // Act
        var handler = new DeleteDocumentHandler(logger.Object, organization.Object);
        var result = await handler.Handle(new DeleteDocument(id, meetingId), default);

        // Assert
        result.Should().NotBeNull();
        bucket.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), CancellationToken.None), Times.Once);
        logger.VerifyLog(LogLevel.Information);
    }
}