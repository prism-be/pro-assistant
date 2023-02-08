// -----------------------------------------------------------------------
//  <copyright file = "CrudTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public static class CrudTests
{
    public static async Task FindMany<TController, TModel>(Func<TController, Task<ActionResult<List<TModel>>>> action)
        where TModel : IDataModel
    {
        // Arrange
        var service = new Mock<ICrudService>();
        service.Setup(x => x.FindMany<TModel>())
            .ReturnsAsync(new List<TModel>());

        // Act
        var controller = (TController)Activator.CreateInstance(typeof(TController), service.Object)!;
        var result = await action(controller);

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        service.Verify(x => x.FindMany<TModel>(), Times.Once);
    }

    public static async Task FindOne<TController, TModel>(Func<TController, Task<ActionResult<TModel>>> action)
        where TModel : IDataModel
    {
        // Arrange
        var service = new Mock<ICrudService>();
        service.Setup(x => x.FindOne<TModel>(It.IsAny<string>()))
            .ReturnsAsync(Activator.CreateInstance<TModel>());

        // Act
        var controller = (TController)Activator.CreateInstance(typeof(TController), service.Object)!;
        var result = await action(controller);

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        service.Verify(x => x.FindOne<TModel>(It.IsAny<string>()), Times.Once);
    }

    public static async Task RemoveOne<TController, TModel>(Func<TController, Task> action)
        where TModel : IDataModel
    {
        // Arrange
        var service = new Mock<ICrudService>();

        // Act
        var controller = (TController)Activator.CreateInstance(typeof(TController), service.Object)!;
        await action(controller);

        // Assert
        service.Verify(x => x.RemoveOne<TModel>(It.IsAny<string>()), Times.Once);
    }

    public static async Task UpsertMany<TController, TModel>(Func<TController, Task> action, Action<Mock<ICrudService>>? setup = null)
        where TModel : IDataModel
    {
        // Arrange
        var service = new Mock<ICrudService>();
        service.Setup(x => x.UpsertMany(It.IsAny<List<TModel>>()))
            .ReturnsAsync(new List<UpsertResult>
            {
                new(Identifier.GenerateString(), Identifier.GenerateString())
            });

        if (setup != null)
        {
            setup(service);
        }

        // Act
        var controller = (TController)Activator.CreateInstance(typeof(TController), service.Object)!;
        await action(controller);

        // Assert
        service.Verify(x => x.UpsertMany(It.IsAny<List<TModel>>()), Times.AtLeastOnce);
    }

    public static async Task UpsertOne<TController, TModel>(Func<TController, Task<ActionResult<UpsertResult>>> action, Action<Mock<ICrudService>>? setup = null)
        where TModel : IDataModel
    {
        // Arrange
        var service = new Mock<ICrudService>();
        service.Setup(x => x.UpsertOne(It.IsAny<TModel>()))
            .ReturnsAsync(new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));

        if (setup != null)
        {
            setup(service);
        }

        // Act
        var controller = (TController)Activator.CreateInstance(typeof(TController), service.Object)!;
        var result = await action(controller);

        // Assert
        result.Result.Should().BeAssignableTo<OkObjectResult>();
        service.Verify(x => x.UpsertOne(It.IsAny<TModel>()), Times.Once);
    }
}