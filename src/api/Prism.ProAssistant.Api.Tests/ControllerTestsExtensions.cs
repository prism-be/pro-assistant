// -----------------------------------------------------------------------
//  <copyright file = "ControllerTestsExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Prism.ProAssistant.Api.Tests;

public static class ControllerTestsExtensions
{
    public static void Validate<T>(ActionResult<T>? result, Action<T> validate) where T : class
    {
        result.Should().NotBeNull();
        var resultObject = result?.Result as OkObjectResult;
        resultObject.Should().NotBeNull();
        var objectValue = resultObject!.Value as T;
        objectValue.Should().NotBeNull();

        validate(objectValue!);
    }
}