namespace Prism.ProAssistant.Domain.Tests.Accounting;

using Core;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;

public class AccountingDocumentAggregatorTests
{
    [Fact]
    public async Task Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();

        // Act and assert
        var aggregator = new AccountingDocumentAggregator();
        aggregator.Init(streamId);

        var accountingDocumentCreated = new AccountingDocumentCreated
        {
            StreamId = streamId,
            Amount = 100,
            Date = DateTime.UtcNow,
            Title = "Title"
        };

        var accountingDocumentUpdated = new AccountingDocumentUpdated
        {
            StreamId = streamId,
            Amount = 200,
            Date = DateTime.UtcNow,
            Title = "Title"
        };

        var accountingDocumentDeleted = new AccountingDocumentDeleted
        {
            StreamId = streamId
        };

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentCreated));
        Assert.NotNull(aggregator.State);
        Assert.Equal(accountingDocumentCreated.Amount, aggregator.State!.Amount);
        Assert.Equal(accountingDocumentCreated.Date, aggregator.State!.Date);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentUpdated));
        Assert.NotNull(aggregator.State);
        Assert.Equal(accountingDocumentUpdated.Amount, aggregator.State!.Amount);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentDeleted));
        Assert.Null(aggregator.State);
    }
}