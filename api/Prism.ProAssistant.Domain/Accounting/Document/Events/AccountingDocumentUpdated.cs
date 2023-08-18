namespace Prism.ProAssistant.Domain.Accounting.Document.Events;

using Core.Attributes;

[StreamType(Streams.AccountingDocument)]
public class AccountingDocumentUpdated: BaseEvent
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
}