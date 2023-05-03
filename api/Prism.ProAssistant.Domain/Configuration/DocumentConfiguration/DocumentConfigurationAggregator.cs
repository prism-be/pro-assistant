using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;

namespace Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;

public class DocumentConfigurationAggregator: IDomainAggregator<DocumentConfiguration>
{

    public void Init(string id)
    {
        State = new DocumentConfiguration
        {
            Id = id
        };
    }

    public DocumentConfiguration? State { get; set; }
    
    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(DocumentConfigurationCreated):
                State = @event.ToEvent<DocumentConfigurationCreated>().DocumentConfiguration;
                break;
            case nameof(DocumentConfigurationUpdated):
                State = @event.ToEvent<DocumentConfigurationUpdated>().DocumentConfiguration;
                break;
            case nameof(DocumentConfigurationDeleted):
                State = null;
                break;
            default:
                throw new NotSupportedException($"The event type {@event.Type} is not implemented");
        }
        
        return Task.CompletedTask;
    }

    public Task Complete()
    {
        return Task.CompletedTask;
    }
}