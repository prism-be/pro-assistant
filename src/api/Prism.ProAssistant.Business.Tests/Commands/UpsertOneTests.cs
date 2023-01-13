// -----------------------------------------------------------------------
//  <copyright file = "UpsertOneTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Commands;

public class UpsertOneTests
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

        var logger = new Mock<ILogger<UpsertOneHandler<Appointment>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var collection = new Mock<IMongoCollection<Appointment>>();
        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(collection.Object);
        collection.Setup(x => x.InsertOneAsync(It.IsAny<Appointment>(), null, CancellationToken.None))
            .Callback(() => appointment.Id = id);

        // Act
        var request = new UpsertOne<Appointment>(appointment);
        var handler = new UpsertOneHandler<Appointment>(logger.Object, organizationContext.Object, userContextAccessor.Object);
        var result = await handler.Handle(request, CancellationToken.None);

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

        var logger = new Mock<ILogger<UpsertOneHandler<Appointment>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var collection = new Mock<IMongoCollection<Appointment>>();
        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(collection.Object);

        collection.Setup(x =>
                x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<Appointment>(), It.IsAny<FindOneAndReplaceOptions<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        // Act
        var request = new UpsertOne<Appointment>(appointment);
        var handler = new UpsertOneHandler<Appointment>(logger.Object, organizationContext.Object, userContextAccessor.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Appointment>>(), appointment, It.IsAny<FindOneAndReplaceOptions<Appointment>>(), CancellationToken.None));
        result.Id.Should().Be(id);
    }
}