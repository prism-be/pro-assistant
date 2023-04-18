namespace Prism.ProAssistant.Domain;

public interface IDomainEvent
{
    public string StreamId { get; }
}