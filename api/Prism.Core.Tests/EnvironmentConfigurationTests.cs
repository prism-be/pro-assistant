using FluentAssertions;
using Prism.Core.Exceptions;

namespace Prism.Core.Tests;

public class EnvironmentConfigurationTests
{

    [Fact]
    public void GetConfiguration_Null()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST", null);

        // Act
        var value = EnvironmentConfiguration.GetConfiguration("TEST");

        // Assert
        value.Should().BeNull();
    }

    [Fact]
    public void GetConfiguration_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST", Identifier.GenerateString());

        // Act
        var value = EnvironmentConfiguration.GetConfiguration("TEST");

        // Assert
        value.Should().NotBeNull();
    }

    [Fact]
    public void GetMandatoryConfiguration_Null()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST", null);

        // Act
        var act = () => EnvironmentConfiguration.GetMandatoryConfiguration("TEST");

        // Assert
        act.Should().Throw<MissingConfigurationException>();
    }

    [Fact]
    public void GetMandatoryConfiguration_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST", Identifier.GenerateString());

        // Act
        var value = EnvironmentConfiguration.GetMandatoryConfiguration("TEST");

        // Assert
        value.Should().NotBeNull();
    }
}