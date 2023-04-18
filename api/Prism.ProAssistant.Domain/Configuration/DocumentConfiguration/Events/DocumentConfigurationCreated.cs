namespace Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;

public class DocumentConfigurationCreated: IDomainEvent
{
    required public DocumentConfiguration DocumentConfiguration { get; set; }
    public string StreamId => DocumentConfiguration.Id;
}