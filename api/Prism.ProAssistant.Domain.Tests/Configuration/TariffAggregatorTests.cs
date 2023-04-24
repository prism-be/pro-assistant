using System.Diagnostics;
using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;

namespace Prism.ProAssistant.Domain.Tests.Configuration;

public class TariffAggregatorTests
{
    [Fact]
    public void Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();

        // Act
        var aggregator = new TariffAggregator();
        aggregator.Init(streamId);

        var tariffCreated = new TariffCreated
        {
            Tariff = new Tariff
            {
                Id = streamId,
                Name = "Name"
            }
        };

        var tariffUpdated = new TariffUpdated
        {
            Tariff = new Tariff
            {
                Id = streamId,
                Name = "NameUpdated"
            }
        };

        // Act and assert events
        aggregator.When(DomainEvent.FromEvent(streamId, userId, tariffCreated));
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Name.Should().Be("Name");

        aggregator.When(DomainEvent.FromEvent(streamId, userId, tariffUpdated));
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Name.Should().Be("NameUpdated");
    }
}