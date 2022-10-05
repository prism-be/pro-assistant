// -----------------------------------------------------------------------
//  <copyright file = "EnvironmentConfigurationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Exceptions;

namespace Prism.ProAssistant.Tests.Business;

public class EnvironmentConfigurationTests
{

    [Fact]
    public void GetConfiguration_Empty()
    {
        // Arrange
        var variableKey = Guid.NewGuid().ToString();

        // Act
        var value = EnvironmentConfiguration.GetConfiguration(variableKey);

        // Assert
        value.Should().BeNull();
    }

    [Fact]
    public void GetConfiguration_Ok()
    {
        // Arrange
        var variableKey = Guid.NewGuid().ToString();
        var variableValue = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(variableKey, variableValue);

        // Act
        var value = EnvironmentConfiguration.GetConfiguration(variableKey);

        // Assert
        value.Should().Be(variableValue);
    }

    [Fact]
    public void GetMandatoryConfiguration_Missing()
    {
        // Arrange
        var variableKey = Guid.NewGuid().ToString();

        // Act
        var ex = Assert.Throws<MissingConfigurationException>(() => EnvironmentConfiguration.GetMandatoryConfiguration(variableKey));

        // Assert
        ex.MissingConfigurationKey.Should().Be(variableKey);
    }

    [Fact]
    public void GetMandatoryConfiguration_Ok()
    {
        // Arrange
        var variableKey = Guid.NewGuid().ToString();
        var variableValue = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(variableKey, variableValue);

        // Act
        var value = EnvironmentConfiguration.GetMandatoryConfiguration(variableKey);

        // Assert
        value.Should().Be(variableValue);
    }
}