﻿using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;

namespace Prism.ProAssistant.Domain.Tests.DayToDay;

public class ContactAggregatorTests
{
    [Fact]
    public async Task Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        // Act
        var aggregator = new ContactAggregator();
        aggregator.Init(streamId);
        
        var contactCreated = new ContactCreated
        {
            Contact = new Contact
            {
                Id = streamId,
                FirstName = "John",
                LastName = "Doe"
            }
        };
        
        var contactUpdated = new ContactUpdated
        {
            Contact = new Contact
            {
                Id = streamId,
                FirstName = "Jane",
                LastName = "Doe"
            }
        };
        
        // Act and assert events
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, contactCreated));
        aggregator.State.FirstName.Should().Be("John");
        aggregator.State.LastName.Should().Be("Doe");
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, contactUpdated));
        aggregator.State.FirstName.Should().Be("Jane");
        aggregator.State.LastName.Should().Be("Doe");

        // Assert
        await aggregator.Complete();
        aggregator.State.Id.Should().Be(streamId);
        
        // Assert unknown events
        await aggregator.Invoking(x => x.When(DomainEvent.FromEvent(streamId, userId, new DummyEvent())))
            .Should().ThrowAsync<NotSupportedException>();
    }
}