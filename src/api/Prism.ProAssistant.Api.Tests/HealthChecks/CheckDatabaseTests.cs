// -----------------------------------------------------------------------
//  <copyright file = "CheckDatabaseTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.HealthChecks;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.HealthChecks;

public class CheckDatabaseTests
{
    [Fact]
    public async Task Check_Healthy()
    {
        // Arrange
        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock.Setup(x => x.RunCommand(It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), CancellationToken.None))
            .Returns(new BsonDocument(new List<KeyValuePair<string, object>>
            {
                new("ok", 1)
            }));

        var mongoClient = new Mock<IMongoClient>();
        mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
            .Returns(databaseMock.Object);

        // Act
        var check = new CheckDatabase(mongoClient.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Check_Unhealthy()
    {
        // Arrange
        var databaseMock = new Mock<IMongoDatabase>();
        databaseMock.Setup(x => x.RunCommand(It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), CancellationToken.None))
            .Returns(new BsonDocument(new List<KeyValuePair<string, object>>
            {
                new("ok", 0)
            }));

        var mongoClient = new Mock<IMongoClient>();
        mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
            .Returns(databaseMock.Object);

        // Act
        var check = new CheckDatabase(mongoClient.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}