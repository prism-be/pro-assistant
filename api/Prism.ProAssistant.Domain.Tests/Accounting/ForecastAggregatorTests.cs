namespace Prism.ProAssistant.Domain.Tests.Accounting;

using Core;
using Domain.Accounting.Forecast;
using Domain.Accounting.Forecast.Events;
using FluentAssertions;

public class ForecastAggregatorTests
{
    [Fact]
    public async Task Happy_Ok()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        // Act and assert
        var aggregator = new ForecastAggregator();
        aggregator.Init(streamId);
        
        var forecastCreated = new ForecastCreated
        {
            StreamId = streamId,
            Title = Identifier.GenerateString()
        };
        
        var forecastUpdated = new ForecastUpdated
        {
            Title = Identifier.GenerateString()
        };
        
        var forecastDeleted = new ForecastDeleted
        {
            StreamId = streamId
        };
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastCreated));
        aggregator.State.Should().NotBeNull();
        aggregator.State!.Title.Should().Be(forecastCreated.Title);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastUpdated));
        aggregator.State.Should().NotBeNull();
        aggregator.State!.Title.Should().Be(forecastUpdated.Title);
        
        await aggregator.Complete();
        aggregator.State.Should().NotBeNull();
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastDeleted));
        aggregator.State.Should().BeNull();
    }
}