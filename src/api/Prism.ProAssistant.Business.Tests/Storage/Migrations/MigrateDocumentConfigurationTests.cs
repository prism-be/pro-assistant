// -----------------------------------------------------------------------
//  <copyright file = "MigrateDocumentConfigurationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Business.Storage.Migrations;

namespace Prism.ProAssistant.Business.Tests.Storage.Migrations;

public class MigrateDocumentConfigurationTests
{
    [Fact]
    public async Task Migrate_NoMigrate()
    {
        // Arrange
        var logger = new Mock<ILogger<MigrateDocumentConfiguration>>();

        var collection = new Mock<IMongoCollection<DocumentConfiguration>>();
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<DocumentConfiguration>.Empty, null, CancellationToken.None))
            .ReturnsAsync(0);
        
        var database = new Mock<IMongoDatabase>();
        database.Setup(x => x.GetCollection<DocumentConfiguration>("documents-configuration", null))
            .Returns(collection.Object);
            
        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.Setup(x => x.Database).Returns(database.Object);

        // Act
        var migrator = new MigrateDocumentConfiguration(organizationContext.Object, logger.Object);
        await migrator.MigrateAsync();

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
        
        database.Verify(x => x.RenameCollectionAsync("documents", "documents-configuration", null, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Migrate_NoMigration()
    {
        // Arrange
        var logger = new Mock<ILogger<MigrateDocumentConfiguration>>();

        var collection = new Mock<IMongoCollection<DocumentConfiguration>>();
        collection.Setup(x => x.CountDocumentsAsync(FilterDefinition<DocumentConfiguration>.Empty, null, CancellationToken.None))
            .ReturnsAsync(42);
        
        var database = new Mock<IMongoDatabase>();
        database.Setup(x => x.GetCollection<DocumentConfiguration>("documents-configuration", null))
            .Returns(collection.Object);
            
        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.Setup(x => x.Database).Returns(database.Object);

        // Act
        var migrator = new MigrateDocumentConfiguration(organizationContext.Object, logger.Object);
        await migrator.MigrateAsync();

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
        
        database.Verify(x => x.RenameCollectionAsync("documents", "documents-configuration", null, CancellationToken.None), Times.Never);
    }
}