// -----------------------------------------------------------------------
//  <copyright file = "LogCommandsBehaviorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Tests;
using Prism.ProAssistant.Business.Behaviors;

namespace Prism.ProAssistant.Business.Tests.Behaviors;

public class LogCommandsBehaviorTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<LogCommandsBehavior<DummyRequest, DummyResponse>>>();

        var logCommandsBehavior = new LogCommandsBehavior<DummyRequest, DummyResponse>(logger.Object);

        // Act and Assert
        var loginRequest = new DummyRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        await logCommandsBehavior.Handle(loginRequest, Mock.Of<RequestHandlerDelegate<DummyResponse>>(), CancellationToken.None);

        // Assert
        logger.Invocations.Should().NotBeEmpty();
    }
}