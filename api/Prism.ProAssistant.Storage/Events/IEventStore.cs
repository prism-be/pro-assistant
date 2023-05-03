namespace Prism.ProAssistant.Storage.Events;

using Domain;

public interface IEventStore
{
    
    Task<UpsertResult> Persist<T>(string streamId);
    Task Raise(BaseEvent eventData);
    Task<UpsertResult> RaiseAndPersist<T>(BaseEvent eventData);
}