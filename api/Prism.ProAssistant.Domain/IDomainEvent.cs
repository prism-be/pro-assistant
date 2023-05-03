namespace Prism.ProAssistant.Domain;

using Core.Attributes;

public abstract class BaseEvent
{
    public virtual string StreamId { get; set; } = "unknown";
    public string StreamType => StreamTypeAttribute.GetStreamType(GetType());
}