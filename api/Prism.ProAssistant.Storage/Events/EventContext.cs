namespace Prism.ProAssistant.Storage.Events;

using Domain;
using Infrastructure.Authentication;

public class EventContext
{
    required public DomainEvent Event { get; set; }
    required public UserOrganization Context { get; set; }
}