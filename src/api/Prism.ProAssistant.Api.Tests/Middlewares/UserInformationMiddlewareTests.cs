// -----------------------------------------------------------------------
//  <copyright file = "UserInformationMiddlewareTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Middlewares;

public class UserInformationMiddlewareTests
{

    [Fact]
    public async Task Anonymous_Ok()
    {
        // Arrange
        var name = Identifier.GenerateString();
        var id = Identifier.GenerateString();
        var organizationId = Identifier.GenerateString();

        Task Next(HttpContext context)
        {
            return Task.FromResult(true);
        }

        var user = new User();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new(ClaimTypes.NameIdentifier, id),
                    new Claim("name", name),
                    new Claim("extension_Organization", organizationId)
                }, "TestAuthType")
            )
        };

        // Act
        var middelware = new UserInformationMiddleware(Next);
        await middelware.InvokeAsync(context, user);

        // Assert
        user.IsAuthenticated.Should().BeTrue();
        user.Id.Should().Be(id);
        user.Organization.Should().Be(organizationId);
        user.Name.Should().Be(name);
    }

    [Fact]
    public async Task Invoke_Ok()
    {
        // Arrange
        var name = Identifier.GenerateString();
        var id = Identifier.GenerateString();
        var organizationId = Identifier.GenerateString();

        Task Next(HttpContext context)
        {
            return Task.FromResult(true);
        }

        var user = new User();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new(ClaimTypes.NameIdentifier, id),
                    new Claim("name", name),
                    new Claim("extension_Organization", organizationId)
                }, "TestAuthType")
            )
        };

        // Act
        var middelware = new UserInformationMiddleware(Next);
        await middelware.InvokeAsync(context, user);

        // Assert
        user.IsAuthenticated.Should().BeTrue();
        user.Id.Should().Be(id);
        user.Organization.Should().Be(organizationId);
        user.Name.Should().Be(name);
    }
}