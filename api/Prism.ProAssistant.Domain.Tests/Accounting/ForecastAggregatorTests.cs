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
    public async Task ComputeBudget_Daily()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        var dailyPrevisionExpense = new ForecastPrevision
        {
            Id = Identifier.GenerateString(),
            Amount = 10,
            Type = ForecastPrevisionType.Expense,
            RecurringCount = 1,
            RecurringType = RecurringType.Daily,
            Name = "Daily from 5",
            StartDate = new DateTime(2023, 1, 5),
            EndDate = new DateTime(2023, 5, 14)
        };
        
        var dailyPrevisionIncome = new ForecastPrevision
        {
            Id = Identifier.GenerateString(),
            Amount = 50,
            Type = ForecastPrevisionType.Income,
            RecurringCount = 1,
            RecurringType = RecurringType.Daily,
            Name = "Daily from 1",
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2023, 5, 14)
        };
        
        // Act
        var aggregator = new ForecastAggregator();
        aggregator.Init(streamId);
        aggregator.State!.Year = 2023;
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, new ForecastPrevisionCreated { Prevision = dailyPrevisionExpense }));
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, new ForecastPrevisionCreated { Prevision = dailyPrevisionIncome }));
        await aggregator.Complete();
        
        // Assert
        aggregator.State.Should().NotBeNull();
        aggregator.State!.WeeklyBudgets.Should().HaveCount(53);
        aggregator.State!.WeeklyBudgets[0].Amount.Should().Be(50);
        aggregator.State!.WeeklyBudgets[1].Amount.Should().Be(7 * 50 - 4 * 10);
        aggregator.State!.WeeklyBudgets[2].Amount.Should().Be(7 * 50 - 7 * 10);
        aggregator.State!.WeeklyBudgets.Sum(x => x.Amount).Should().Be(134 * 50 - 130 * 10);
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