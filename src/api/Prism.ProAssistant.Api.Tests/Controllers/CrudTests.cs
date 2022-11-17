// -----------------------------------------------------------------------
//  <copyright file = "CrudTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Api.Tests.Controllers
{
    public static class CrudTests
    {
        public static async Task FindOne<TController, TModel>(Func<TController, Task<ActionResult<TModel>>> action)
            where TModel : IDataModel, new()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<FindOne<TModel>>(), CancellationToken.None))
                .ReturnsAsync(new TModel());

            // Act
            var controller = (TController)Activator.CreateInstance(typeof(TController), mediator.Object)!;
            var result = await action(controller);

            // Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<FindOne<TModel>>(), CancellationToken.None), Times.Once);
        }
        
        public static async Task UpsertOne<TController, TModel>(Func<TController, Task<ActionResult<UpsertResult>>> action, Action<Mock<IMediator>>? setup = null)
            where TModel : IDataModel, new()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpsertOne<TModel>>(), CancellationToken.None))
                .ReturnsAsync(new UpsertResult(Identifier.GenerateString()));

            if (setup != null)
            {
                setup(mediator);
            }

            // Act
            var controller = (TController)Activator.CreateInstance(typeof(TController), mediator.Object)!;
            var result = await action(controller);

            // Assert
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            mediator.Verify(x => x.Send(It.IsAny<UpsertOne<TModel>>(), CancellationToken.None), Times.Once);
        }
    }
}