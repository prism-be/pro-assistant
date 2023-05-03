namespace Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;

using Core.Attributes;

[StreamType(Streams.DocumentsConfiguration)]
public class DocumentConfigurationUpdated: BaseEvent
{
    required public DocumentConfiguration DocumentConfiguration { get; set; }
    public override string StreamId => DocumentConfiguration.Id;
}