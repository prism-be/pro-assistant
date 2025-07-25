using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Core;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Storage.Users;

namespace Prism.ProAssistant.Storage.Tests.Users;

public class UserOrganizationServiceTests
{

    [Fact]
    public async Task GetGetUserOrganization_Unauthenticated()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var result = await service.GetUserOrganization();

        // Assert
        result.Should().Be("demo");
    }

    [Fact]
    public void GetUserId_Ok()
    {
        // Arrange
        var userId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var result = service.GetUserId();
        var name = service.GetName();

        // Assert
        result.Should().Be(userId);
        name.Should().Be("Test User");
    }

    [Fact]
    public void GetUserId_Unauthenticated()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var result = service.GetUserId();
        var name = service.GetName();

        // Assert
        result.Should().BeNull();
        name.Should().BeNull();
    }
    
    [Fact]
    public async Task GetUserOrganization_NotFound()
    {
        // Arrange
        var userId = Identifier.GenerateString();
        var organization = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync($"organization-{userId}", CancellationToken.None)).ReturnsAsync(null as byte[]);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User"),
                new Claim(ClaimTypes.NameIdentifier, string.Empty)
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();
        var container = new Mock<IStateContainer<UserOrganization>>();
        container.Setup(x => x.ReadAsync(userId)).ReturnsAsync(new UserOrganization
        {
            Organization = organization
        });
        globalStateProvider.Setup(x => x.GetGlobalContainerAsync<UserOrganization>()).ReturnsAsync(container.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var act = async () => await service.GetUserOrganization();

        // Assert
        var ex = await act.Should().ThrowAsync<NotFoundException>();
        ex.Which.Message.Should().Be("The collection was not found because the user has no id.");
    }

    [Fact]
    public async Task GetUserOrganization_Ok()
    {
        // Arrange
        var userId = Identifier.GenerateString();
        var organization = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync($"organization-{userId}", CancellationToken.None)).ReturnsAsync(null as byte[]);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();
        var container = new Mock<IStateContainer<UserOrganization>>();
        container.Setup(x => x.ReadAsync(userId)).ReturnsAsync(new UserOrganization
        {
            Organization = organization
        });
        globalStateProvider.Setup(x => x.GetGlobalContainerAsync<UserOrganization>()).ReturnsAsync(container.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var result = await service.GetUserOrganization();

        // Assert
        result.Should().Be(organization);
    }

    [Fact]
    public async Task GetUserOrganization_Ok_UserNotFound()
    {
        // Arrange
        var userId = Identifier.GenerateString();
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.GetAsync($"organization-{userId}", CancellationToken.None)).ReturnsAsync(null as byte[]);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test Authentication Type"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var logger = new Mock<ILogger<UserOrganizationService>>();

        var globalStateProvider = new Mock<IGlobalStateProvider>();
        var container = new Mock<IStateContainer<UserOrganization>>();
        globalStateProvider.Setup(x => x.GetGlobalContainerAsync<UserOrganization>()).ReturnsAsync(container.Object);

        // Act
        var service = new UserOrganizationService(logger.Object, httpContextAccessor.Object, cache.Object);
        var result = await service.GetUserOrganization();

        // Assert
        result.Should().Be("demo");
    }
}