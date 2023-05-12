namespace Prism.ProAssistant.Domain.Tests.Accounting;

using Core;
using Domain.Accounting.Forecast;
using Domain.Accounting.Forecast.Events;
using FluentAssertions;

public class ForecastAggregatorTests
{
    [Fact]
    public async Task HappyPrevisions_Ok()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        // Act and assert
        var aggregator = new ForecastAggregator();
        aggregator.Init(streamId);

        var forecastPrevisionCreated = new ForecastPrevisionCreated
        {
            Prevision = new ForecastPrevision
            {
                Id = Identifier.GenerateString(),
                Amount = 42
            }
        };
        
        var forecastPrevisionUpdated = new ForecastPrevisionUpdated
        {
            Prevision = new ForecastPrevision
            {
                Id = forecastPrevisionCreated.Prevision.Id,
                Amount = 43
            }
        };
        
        var forecastPrevisionDeleted = new ForecastPrevisionDeleted
        {
            Id = forecastPrevisionCreated.Prevision.Id
        };
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastPrevisionCreated));
        aggregator.State.Should().NotBeNull();
        aggregator.State!.Previsions.Should().HaveCount(1);
        aggregator.State!.Previsions[0].Amount.Should().Be(forecastPrevisionCreated.Prevision.Amount);
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastPrevisionUpdated));
        aggregator.State.Should().NotBeNull();
        aggregator.State!.Previsions.Should().HaveCount(1);
        aggregator.State!.Previsions[0].Amount.Should().Be(forecastPrevisionUpdated.Prevision.Amount);
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, forecastPrevisionDeleted));
        aggregator.State.Should().NotBeNull();
        aggregator.State!.Previsions.Should().HaveCount(0);
    }
    
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