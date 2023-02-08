// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AuthenticationControllerTests
{

    [Fact]
    public void GetUser_Anonymous()
    {
        // Arrange
        var user = new Mock<User>();
        user.Setup(x => x.Name).Returns(string.Empty);
        user.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var controller = new AuthenticationController(user.Object);
        var result = controller.GetUser();

        // Assert
        ControllerTestsExtensions.Validate(result, x =>
        {
            x.Authenticated.Should().BeFalse();
            x.Name.Should().BeNull();
        });
    }

    [Fact]
    public void GetUser_Ok()
    {
        // Arrange
        var name = Identifier.GenerateString();

        var user = new Mock<User>();
        user.Setup(x => x.Name).Returns(name);
        user.Setup(x => x.IsAuthenticated).Returns(true);

        // Act
        var controller = new AuthenticationController(user.Object);
        var result = controller.GetUser();

        // Assert
        ControllerTestsExtensions.Validate(result, x =>
        {
            x.Authenticated.Should().BeTrue();
            x.Name.Should().BeEquivalentTo(name);
        });
    }
}