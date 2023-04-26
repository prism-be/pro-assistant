using FluentAssertions;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.ProAssistant.Api.Controllers;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public void GetUser_Ok()
    {
        // Arrange
        var userOrganization = new UserOrganization
        {
            Name = Identifier.GenerateString(),
            Organization = Identifier.GenerateString(),
        };
        
        // Act
        var authenticationController = new AuthenticationController(userOrganization);
        var result = authenticationController.GetUser();

        // Assert
        result.Should().NotBeNull();
        result.IsAuthenticated.Should().BeTrue();
        result.Name.Should().Be(userOrganization.Name);
        result.Organization.Should().Be(userOrganization.Organization);
    }
}