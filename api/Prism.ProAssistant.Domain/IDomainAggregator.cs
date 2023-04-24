namespace Prism.ProAssistant.Domain;

public interface IDomainAggregator<T>
{
    public void Init(string id);
    public T? State { get; }
    public void When(DomainEvent @event);
}