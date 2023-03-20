// -----------------------------------------------------------------------
//  <copyright file = "HostBuilderExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Insights;
using Serilog;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Extensions;

public class HostBuilderExtensionsTests
{

    [Fact]
    public void AddApplicationInsights_Ok()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddApplicationInsights();

        // Assert
        builder.Services.Should().Contain(x => x.ServiceType == typeof(ITelemetryInitializer));
        builder.Services.Should().Contain(x => x.ImplementationType == typeof(RoleNameInitializer));
    }

    [Fact]
    public void AddSerilog_Ok()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddSerilog();

        // Assert
        builder.Services.Should().Contain(x => x.ServiceType == typeof(IDiagnosticContext));
    }
}