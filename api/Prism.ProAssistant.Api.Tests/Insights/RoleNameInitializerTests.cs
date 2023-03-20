using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using Prism.ProAssistant.Api.Insights;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Insights;

public class RoleNameInitializerTests
{
    [Fact]
    public void Initialize_Ok()
    {
        // Arrange
        var roleName = Identifier.GenerateString();
        var item = new DependencyTelemetry();
        Environment.SetEnvironmentVariable("ENVIRONMENT", roleName);

        // Act
        var initializer = new RoleNameInitializer();
        initializer.Initialize(item);

        // Assert
        item.Context.Cloud.RoleName.Should().Be(roleName);
    }
}