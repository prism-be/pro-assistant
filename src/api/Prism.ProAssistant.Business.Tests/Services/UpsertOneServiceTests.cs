// -----------------------------------------------------------------------
//  <copyright file = "UpsertOneServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Services;

public class UpsertOneServiceTests
{

    [Fact]
    public async Task UpsertOneHandler_New()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var appointment = new Appointment
        {
            Id = string.Empty,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var logger = new Mock<ILogger<UpsertOneService>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var user = new Mock<User>();

        var collection = new Mock<IMongoCollection<Appointment>>();
        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(collection.Object);
        collection.Setup(x => x.InsertOneAsync(It.IsAny<Appointment>(), null, CancellationToken.None))
            .Callback(() => appointment.Id = id);

        var publisher = new Mock<IPropertyUpdatePublisher>();

        // Act
        var handler = new UpsertOneService(logger.Object, organizationContext.Object, user.Object, publisher.Object);
        var result = await handler.Upsert(appointment);

        // Assert
        collection.Verify(x => x.InsertOneAsync(appointment, null, CancellationToken.None));
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task UpsertOneHandler_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var appointment = new Appointment
        {
            Id = id,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var logger = new Mock<ILogger<UpsertOneService>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var user = new Mock<User>();

        var collection = new Mock<IMongoCollection<Appointment>>();
        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(collection.Object);

        collection.Setup(x =>
                x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<Appointment>(), It.IsAny<FindOneAndReplaceOptions<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        var publisher = new Mock<IPropertyUpdatePublisher>();

        // Act
        var handler = new UpsertOneService(logger.Object, organizationContext.Object, user.Object, publisher.Object);
        var result = await handler.Upsert(appointment);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Appointment>>(), appointment, It.IsAny<FindOneAndReplaceOptions<Appointment>>(), CancellationToken.None));
        result.Id.Should().Be(id);
    }
}