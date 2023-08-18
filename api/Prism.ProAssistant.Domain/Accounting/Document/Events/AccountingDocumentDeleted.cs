namespace Prism.ProAssistant.Domain.Accounting.Document.Events;

using Core.Attributes;

[StreamType(Streams.AccountingDocument)]
public class AccountingDocumentDeleted: BaseEvent
{
    public DateTime Date { get; set; }
}