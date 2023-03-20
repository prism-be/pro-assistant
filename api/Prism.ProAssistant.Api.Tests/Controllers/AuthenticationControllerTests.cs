using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task GetUser_Ok()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", "Test User")
            }))
        };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        
        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserOrganization()).ReturnsAsync("Test Organization");

        // Act
        var controller = new AuthenticationController(httpContextAccessor.Object, userOrganizationService.Object);
        var result = await controller.GetUser();

        // Assert
        result.IsAuthenticated.Should().BeTrue();
        result.Name.Should().Be("Test User");
        result.Organization.Should().Be("Test Organization");
    }
}