namespace Prism.ProAssistant.Domain.Accounting.Document.Events;

using Core.Attributes;

[StreamType(Streams.AccountingDocument)]
public class AccountingDocumentCreated : BaseEvent
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Reference { get; set; }
}