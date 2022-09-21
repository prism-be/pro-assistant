// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Users;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task Login_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var controller = new AuthenticationController(mediator.Object);

        // Act
        var request = new AuthenticateUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        await controller.Login(request);

        // Assert
        mediator.Verify(x => x.Send(It.Is<AuthenticateUser>(r => r == request), CancellationToken.None), Times.Once);
    }
}