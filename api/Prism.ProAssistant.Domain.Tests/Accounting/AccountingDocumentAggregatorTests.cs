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
            Document = new AccountingDocument()
            {
                Id = streamId,
                Amount = 100,
                Date = DateTime.UtcNow,
                Title = "Title",
                DocumentNumber = 42,
                Reference = Identifier.GenerateString(),
                Category = "unit-tests"
            }
        };

        var accountingDocumentUpdated = new AccountingDocumentUpdated
        {
            StreamId = streamId,
            Document = new AccountingDocument()
            {
                Id = streamId,
                Amount = 200,
                Date = DateTime.UtcNow,
                Title = "Title",
                DocumentNumber = 42,
                Reference = Identifier.GenerateString(),
                Category = "unit-tests-2"
            }
        };

        var accountingDocumentDeleted = new AccountingDocumentDeleted
        {
            StreamId = streamId
        };

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentCreated));
        Assert.NotNull(aggregator.State);
        Assert.Equal(accountingDocumentCreated.Document.Amount, aggregator.State!.Amount);
        Assert.Equal(accountingDocumentCreated.Document.Date, aggregator.State!.Date);
        Assert.Equal(accountingDocumentCreated.Document.Title, aggregator.State!.Title);
        Assert.Equal(accountingDocumentCreated.Document.DocumentNumber, aggregator.State!.DocumentNumber);
        Assert.Equal(accountingDocumentCreated.Document.Reference, aggregator.State!.Reference);
        Assert.Equal(accountingDocumentCreated.Document.Category, aggregator.State!.Category);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentUpdated));
        Assert.NotNull(aggregator.State);
        Assert.Equal(accountingDocumentUpdated.Document.Amount, aggregator.State!.Amount);
        Assert.Equal(accountingDocumentUpdated.Document.Date, aggregator.State!.Date);
        Assert.Equal(accountingDocumentUpdated.Document.Title, aggregator.State!.Title);
        Assert.Equal(accountingDocumentUpdated.Document.DocumentNumber, aggregator.State!.DocumentNumber);
        Assert.Equal(accountingDocumentUpdated.Document.Reference, aggregator.State!.Reference);
        Assert.Equal(accountingDocumentUpdated.Document.Category, aggregator.State!.Category);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, accountingDocumentDeleted));
        Assert.Null(aggregator.State);
    }
}