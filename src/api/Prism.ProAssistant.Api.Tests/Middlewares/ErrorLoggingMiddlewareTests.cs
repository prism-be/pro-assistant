// -----------------------------------------------------------------------
//  <copyright file = "ErrorLoggingMiddlewareTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Middlewares;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Middlewares;

public class ErrorLoggingMiddlewareTests
{

    [Fact]
    public async Task Invoke_Error()
    {
        // Arrange
        var expectedException = new ArgumentNullException();
        var logger = new Mock<ILogger<ErrorLoggingMiddleware>>();

        Task Next(HttpContext context)
        {
            return Task.FromException(expectedException);
        }

        var httpContext = new DefaultHttpContext();

        var exceptionHandlingMiddleware = new ErrorLoggingMiddleware(Next, logger.Object);

        //act
        var act = async () =>
        {
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<ErrorLoggingMiddleware>>();

        Task Next(HttpContext context)
        {
            return Task.FromResult(true);
        }

        var httpContext = new DefaultHttpContext();

        var exceptionHandlingMiddleware = new ErrorLoggingMiddleware(Next, logger.Object);

        //act
        var act = async () =>
        {
            await exceptionHandlingMiddleware.InvokeAsync(httpContext);
        };

        // Assert
        await act.Should().NotThrowAsync();
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);
    }
}