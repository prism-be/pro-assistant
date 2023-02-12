// -----------------------------------------------------------------------
//  <copyright file = "UpdatePropertyServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;

namespace Prism.ProAssistant.Business.Tests.Services;

public class UpdatePropertyServiceTests
{

    [Fact]
    public async Task Update_Empty()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdatePropertyService>>();
        var user = new Mock<User>();
        var propertyPublisher = new Mock<IPropertyUpdatePublisher>();
        var organizationContext = new Mock<IOrganizationContext>();
        var collection = organizationContext.SetupCollection<Appointment>();

        // Act
        var service = new UpdatePropertyService(logger.Object, organizationContext.Object, propertyPublisher.Object, user.Object);
        await service.Update<Appointment>(string.Empty, "BirtDate", DateTime.Now.ToString(CultureInfo.InvariantCulture));

        // Assert
        collection.Verify(x => x.UpdateOneAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<UpdateDefinition<Appointment>>(), null, CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdatePropertyService>>();
        var user = new Mock<User>();
        var propertyPublisher = new Mock<IPropertyUpdatePublisher>();
        var organizationContext = new Mock<IOrganizationContext>();
        var collection = organizationContext.SetupCollection<Appointment>();
        collection.Setup(x => x.UpdateOneAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<UpdateDefinition<Appointment>>(), null, CancellationToken.None))
            .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, ObjectId.GenerateNewId()));

        // Act
        var service = new UpdatePropertyService(logger.Object, organizationContext.Object, propertyPublisher.Object, user.Object);
        await service.Update<Appointment>(Identifier.GenerateString(), "BirtDate", DateTime.Now.ToString(CultureInfo.InvariantCulture));

        // Assert
        collection.Verify(x => x.UpdateOneAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<UpdateDefinition<Appointment>>(), null, CancellationToken.None), Times.Once);
    }
}