// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
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
        var user = new User
        {
            Name = string.Empty,
            IsAuthenticated = false
        };

        // Act
        var controller = new AuthenticationController(user);
        var result = controller.GetUser();

        // Assert
        ControllerTestsExtensions.Validate(result, x =>
        {
            x.Authenticated.Should().BeFalse();
            x.Name.Should().BeEmpty();
        });
    }

    [Fact]
    public void GetUser_Ok()
    {
        // Arrange
        var name = Identifier.GenerateString();

        var user = new User
        {
            Name = name,
            IsAuthenticated = true
        };

        // Act
        var controller = new AuthenticationController(user);
        var result = controller.GetUser();

        // Assert
        ControllerTestsExtensions.Validate(result, x =>
        {
            x.Authenticated.Should().BeTrue();
            x.Name.Should().BeEquivalentTo(name);
        });
    }
}