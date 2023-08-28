namespace Prism.ProAssistant.Storage.Events;

using Domain;
using Infrastructure.Authentication;

public class EventContext<T>
    
{
    required public DomainEvent Event { get; set; }
    required public UserOrganization Context { get; set; }
    public T? PreviousState { get; set; }
    public T? CurrentState { get; set; }
}