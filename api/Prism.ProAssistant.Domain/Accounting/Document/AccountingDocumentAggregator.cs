namespace Prism.ProAssistant.Domain.Accounting.Document;

using Core;
using Events;

public class AccountingDocumentAggregator : IDomainAggregator<AccountingDocument>
{
    public void Init(string id)
    {
        State = new AccountingDocument
        {
            Id = id
        };
    }

    public AccountingDocument? State { get; private set; } = new()
    {
        Id = Identifier.GenerateString()
    };

    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(AccountingDocumentCreated):
                Apply(@event.ToEvent<AccountingDocumentCreated>());
                break;
            case nameof(AccountingDocumentUpdated):
                Apply(@event.ToEvent<AccountingDocumentUpdated>());
                break;
            case nameof(AccountingDocumentDeleted):
                ApplyDelete();
                break;
        }

        return Task.CompletedTask;
    }

    public Task Complete()
    {
        return Task.CompletedTask;
    }

    private void Apply(AccountingDocumentUpdated @event)
    {
        State = EnsureState();
        State = @event.Document;
        State.Id = @event.StreamId;
    }

    private void Apply(AccountingDocumentCreated @event)
    {
        State = @event.Document;
        State.Id = @event.StreamId;
    }

    private void ApplyDelete()
    {
        State = null;
    }

    private AccountingDocument EnsureState()
    {
        if (State == null)
        {
            throw new InvalidOperationException("The state has not been initialized");
        }

        return State;
    }
}