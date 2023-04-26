using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Middlewares;

namespace Prism.ProAssistant.Api.Tests.Middlewares;

public class NextJsRouterMiddleWareTests
{
    [Fact]
    public async Task Invoke_Api()
    {
        // Arrange
        var logger = new Mock<ILogger<NextJsRouterMiddleWare>>();

        var called = false;
        
        Task Next(HttpContext context)
        {
            called = true;
            return Task.FromResult(true);
        } 
        
        // Act
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Path = "/api"
            }
        };
        var exceptionHandlingMiddleware = new NextJsRouterMiddleWare(Next, logger.Object);
        await exceptionHandlingMiddleware.Invoke(httpContext);

        // Assert
        called.Should().BeTrue();
    }
}