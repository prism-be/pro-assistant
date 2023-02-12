// -----------------------------------------------------------------------
//  <copyright file = "DownloadDocumentServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests;

public class DownloadDocumentServiceTests
{
    [Fact]
    public async Task Handle_Not_Found()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        organization.SetupBucket();

        var logger = new Mock<ILogger<DownloadDocumentService>>();

        // Act
        var handler = new DownloadDocumentService(organization.Object, logger.Object);
        var result = await handler.Download(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        organization.SetupBucket(new GridFSFileInfo(new BsonDocument(new List<KeyValuePair<string, object>>
        {
            new("filename", "test.txt")
        })));

        var logger = new Mock<ILogger<DownloadDocumentService>>();

        // Act
        var handler = new DownloadDocumentService(organization.Object, logger.Object);
        var result = await handler.Download(id);

        // Assert
        result.Should().NotBeNull();
        result?.FileName.Should().Be("test.txt");
    }
}