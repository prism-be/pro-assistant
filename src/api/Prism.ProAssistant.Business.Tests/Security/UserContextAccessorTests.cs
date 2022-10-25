// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Tests.Security;

public class UserContextAccessorTests
{
    [Fact]
    public void Anonymous()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var mediator = new Mock<IMediator>();

        // Act
        var userContextAccessor = new UserContextAccessor(httpContextAccessor.Object, mediator.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeFalse();
        userContextAccessor.UserId.Should().BeEmpty();
        userContextAccessor.Name.Should().BeEmpty();
    }

    [Fact]
    public void Logged_Id_Empty()
    {
        // Arrange
        var name = Identifier.GenerateString();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", name)
            }, "TestAuthType"))
        };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var mediator = new Mock<IMediator>();

        // Act
        var userContextAccessor = new UserContextAccessor(httpContextAccessor.Object, mediator.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeTrue();
        userContextAccessor.UserId.Should().BeEmpty();
        userContextAccessor.Name.Should().Be(name);
    }

    [Fact]
    public void Logged_Name_Empty()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id)
            }, "TestAuthType"))
        };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var mediator = new Mock<IMediator>();

        // Act
        var userContextAccessor = new UserContextAccessor(httpContextAccessor.Object, mediator.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeTrue();
        userContextAccessor.UserId.Should().Be(id);
        userContextAccessor.Name.Should().BeEmpty();
    }

    [Fact]
    public void Logged_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var name = Identifier.GenerateString();

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new(ClaimTypes.NameIdentifier, id),
                    new Claim("name", name)
                }, "TestAuthType")
            )
        };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var mediator = new Mock<IMediator>();

        // Act
        var userContextAccessor = new UserContextAccessor(httpContextAccessor.Object, mediator.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeTrue();
        userContextAccessor.UserId.Should().Be(id);
        userContextAccessor.Name.Should().Be(name);
    }

    [Fact]
    public void OrganisationId_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var organizationId = Identifier.GenerateString();
        var name = Identifier.GenerateString();

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new(ClaimTypes.NameIdentifier, id),
                    new Claim("name", name)
                }, "TestAuthType")
            )
        };
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<GetUserInformation>(), CancellationToken.None))
            .ReturnsAsync(new UserInformation
            {
                Id = id,
                Organizations = new List<Organization>
                {
                    new()
                    {
                        Id = organizationId
                    }
                }
            });

        // Act
        var userContextAccessor = new UserContextAccessor(httpContextAccessor.Object, mediator.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeTrue();
        userContextAccessor.UserId.Should().Be(id);
        userContextAccessor.Name.Should().Be(name);
        userContextAccessor.OrganisationId.Should().Be(organizationId);

        // twice to ckeck Lazy
        userContextAccessor.OrganisationId.Should().Be(organizationId);
        mediator.Verify(x => x.Send(It.IsAny<GetUserInformation>(), CancellationToken.None), Times.Once);
    }
}