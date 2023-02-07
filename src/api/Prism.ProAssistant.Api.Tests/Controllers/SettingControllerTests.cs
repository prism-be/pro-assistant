// -----------------------------------------------------------------------
//  <copyright file = "SettingControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Moq;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class SettingControllerTests
{
    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<SettingController, Setting>(c => c.FindMany());
    }

    [Fact]
    public async Task SaveSettings()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<SaveSettings>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value);

        // Act
        var controller = new SettingController(mediator.Object);
        await controller.SaveSettings(new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Value = Identifier.GenerateString()
            }
        });

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<SaveSettings>(), CancellationToken.None), Times.Once);
    }
}