// -----------------------------------------------------------------------
//  <copyright file = "SettingsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Graph.Settings;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.UnitTesting.Fakes;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph;

public class SettingsTests
{

    [Fact]
    public void Configure_Ok()
    {
        // Arrange
        var descriptor = new Mock<IObjectTypeDescriptor<Setting>>();

        // Act
        SettingType.ConfigureSetting(descriptor.Object);

        // Assert
        descriptor.Invocations.Count.Should().Be(2);
    }

    [Fact]
    public async Task CreateSettingAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var setting = new Setting
        {
            Id = id,
            Value = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake
        {
            SettingsReplace = setting
        };

        // Act
        var query = new SettingMutation();
        var result = await query.UpsertSettingAsync(setting, organisationContext, Mock.Of<ILogger<SettingMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetSettingById_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var existing = new Setting
        {
            Id = id,
            Value = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake();
        await organisationContext.Settings.InsertOneAsync(existing);

        // Act
        var query = new SettingQuery();
        var result = query.GetSettingById(id, organisationContext);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateSettingAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var setting = new Setting
        {
            Id = id,
            Value = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake
        {
            SettingsReplace = setting
        };

        // Act
        var query = new SettingMutation();
        await query.UpsertSettingAsync(setting, organisationContext, Mock.Of<ILogger<SettingMutation>>(), Mock.Of<IUserContextAccessor>());
        var result = await query.UpsertSettingAsync(setting, organisationContext, Mock.Of<ILogger<SettingMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }
}