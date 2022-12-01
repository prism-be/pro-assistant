// -----------------------------------------------------------------------
//  <copyright file = "MigrationControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Storage.Migrations;
using Prism.ProAssistant.UnitTesting.Extensions;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class MigrationControllerTests
{

    [Fact]
    public async Task Migrate_MigrateDocumentConfiguration()
    {
        // Arrange
        var migrator = new Mock<IMigrateDocumentConfiguration>();
        var logger = new Mock<ILogger<MigrationController>>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(migrator.Object);

        // Act
        var controller = new MigrationController(logger.Object, serviceProvider.Object);
        await controller.ExecuteMigration("MigrateDocumentConfiguration");

        // Assert
        logger.VerifyLog(LogLevel.Warning, Times.Never());
        migrator.Verify(x => x.MigrateAsync(), Times.Once);
    }

    [Fact]
    public async Task Migrate_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<MigrationController>>();
        var serviceProvider = new Mock<IServiceProvider>();

        // Act
        var controller = new MigrationController(logger.Object, serviceProvider.Object);
        await controller.ExecuteMigration("plop");

        // Assert
        logger.VerifyLog(LogLevel.Warning, Times.Once());
    }
}