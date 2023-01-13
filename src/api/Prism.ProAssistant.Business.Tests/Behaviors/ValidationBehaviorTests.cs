// -----------------------------------------------------------------------
//  <copyright file = "ValidationBehaviorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Moq;
using Prism.Picshare.Tests;
using Prism.ProAssistant.Business.Behaviors;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Tests.Behaviors;

namespace Prism.ProAssistant.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_Invalid()
    {
        // Arrange
        var validators = new List<IValidator<DummyRequest>>
        {
            new DummyRequestValidator()
        };

        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(validators);

        // Act and Assert
        var loginRequest = new DummyRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var exception = await Assert.ThrowsAsync<InvalidModelException>(async () =>
            await validationBehavior.Handle(loginRequest, Mock.Of<RequestHandlerDelegate<DummyResponse>>(), CancellationToken.None));

        // Assert
        Assert.NotNull(exception);
        Assert.NotEmpty(exception.Validations);
    }

    [Fact]
    public async Task Handle_NoValidators()
    {
        // Arrange
        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(new List<IValidator<DummyRequest>>());

        // Act and Assert
        var loginRequest = new DummyRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, Mock.Of<RequestHandlerDelegate<DummyResponse>>(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var validators = new List<IValidator<DummyRequest>>
        {
            new DummyRequestValidator()
        };

        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(validators);

        // Act
        var loginRequest = new DummyRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, Mock.Of<RequestHandlerDelegate<DummyResponse>>(), CancellationToken.None);
        Assert.Null(result);
    }
}