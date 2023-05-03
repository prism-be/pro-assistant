using System.Diagnostics;
using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;

namespace Prism.ProAssistant.Domain.Tests.Configuration;

public class DocumentConfigurationAggregatorTests
{
    [Fact]
    public async Task Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        // Act and assert
        var aggregator = new DocumentConfigurationAggregator();
        aggregator.Init(streamId);

        var documentConfigurationCreated = new DocumentConfigurationCreated
        {
            DocumentConfiguration = new DocumentConfiguration
            {
                Id = streamId,
                Name = "Name",
            }
        };
        
        var documentConfigurationUpdated = new DocumentConfigurationUpdated
        {
            DocumentConfiguration = new DocumentConfiguration
            {
                Id = streamId,
                Name = "NameUpdated",
            }
        };
        
        var documentConfigurationDeleted = new DocumentConfigurationDeleted
        {
            StreamId = streamId
        };
        
        // Act and assert events
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, documentConfigurationCreated));
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Name.Should().Be("Name");
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, documentConfigurationUpdated));
        await aggregator.Complete();
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.Name.Should().Be("NameUpdated");
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, documentConfigurationDeleted));
        aggregator.State.Should().BeNull();

        // Assert unknown events
        await aggregator.Invoking(x => x.When(DomainEvent.FromEvent(streamId, userId, new DummyEvent())))
            .Should().ThrowAsync<NotSupportedException>();
    }
}