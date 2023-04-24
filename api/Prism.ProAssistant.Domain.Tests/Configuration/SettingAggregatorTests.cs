using System.Diagnostics;
using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Settings.Events;

namespace Prism.ProAssistant.Domain.Tests.Configuration;

public class SettingAggregatorTests
{
    [Fact]
    public void Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();

        // Act
        var aggregator = new SettingAggregator();
        aggregator.Init(streamId);

        var settingCreated = new SettingCreated
        {
            Setting = new Setting
            {
                Id = streamId,
                Value = "Value"
            }
        };

        var settingUpdated = new SettingUpdated
        {
            Setting = new Setting
            {
                Id = streamId,
                Value = "ValueUpdated"
            }
        };

        // Act and assert events
        aggregator.When(DomainEvent.FromEvent(streamId, userId, settingCreated));
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Value.Should().Be("Value");

        aggregator.When(DomainEvent.FromEvent(streamId, userId, settingUpdated));
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Value.Should().Be("ValueUpdated");
    }
}