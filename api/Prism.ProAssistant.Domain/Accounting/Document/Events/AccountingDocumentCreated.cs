namespace Prism.ProAssistant.Domain.Accounting.Document.Events;

using Core.Attributes;

[StreamType(Streams.AccountingDocument)]
public class AccountingDocumentCreated : BaseEvent
{
    public required AccountingDocument Document { get; set; }
}