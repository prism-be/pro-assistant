﻿// -----------------------------------------------------------------------
//  <copyright file = "RemoveOneServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Services;

public class RemoveOneServiceTests
{
    [Fact]
    public async Task RemoveOneHandler_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RemoveOneService>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var user = new Mock<User>();

        var collection = new Mock<IMongoCollection<Appointment>>();
        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(collection.Object);

        // Act
        var handler = new RemoveOneService(logger.Object, organizationContext.Object, user.Object);
        await handler.Remove<Appointment>(Identifier.GenerateString());

        // Assert
        collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Appointment>>(), CancellationToken.None));
    }
}