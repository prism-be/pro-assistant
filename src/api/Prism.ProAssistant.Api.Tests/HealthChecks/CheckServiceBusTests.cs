// -----------------------------------------------------------------------
//  <copyright file = "CheckServiceBusTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Prism.ProAssistant.Api.HealthChecks;
using RabbitMQ.Client;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.HealthChecks;

public class CheckServiceBusTests
{

    [Fact]
    public async Task Check_Healthy()
    {
        // Arrange
        var connection = new Mock<IConnection>();
        connection.Setup(x => x.IsOpen).Returns(true);

        // Act
        var check = new CheckServiceBus(connection.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Check_Unhealthy()
    {
        // Arrange
        var connection = new Mock<IConnection>();
        connection.Setup(x => x.IsOpen).Returns(false);

        // Act
        var check = new CheckServiceBus(connection.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}