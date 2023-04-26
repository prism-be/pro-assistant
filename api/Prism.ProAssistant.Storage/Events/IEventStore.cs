namespace Prism.ProAssistant.Storage.Events;

using Domain;

public interface IEventStore
{
    
    Task<UpsertResult> Persist<T>(string streamId);
    Task Raise(IDomainEvent eventData);
    Task<UpsertResult> RaiseAndPersist<T>(IDomainEvent eventData);
}