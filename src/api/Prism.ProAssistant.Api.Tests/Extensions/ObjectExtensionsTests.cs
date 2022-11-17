// -----------------------------------------------------------------------
//  <copyright file = "ObjectExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void ToActionResult_Ok()
        {
            // Arrange
            var o = new object();
            
            // Act
            var result = o.ToActionResult();

            // Assert
            result.Should().BeAssignableTo<ActionResult<object>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }
        
        [Fact]
        public void ToActionResult_Null()
        {
            // Arrange
            var o = (object)null!;
            
            // Act
            var result = o.ToActionResult();

            // Assert
            result.Should().BeAssignableTo<ActionResult<object>>();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}