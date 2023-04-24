namespace Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;

public class DocumentConfigurationDeleted: IDomainEvent
{
    required public string StreamId { get; set; }
    public string StreamType => "documents-configuration";
}