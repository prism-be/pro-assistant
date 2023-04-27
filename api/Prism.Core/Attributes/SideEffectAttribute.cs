namespace Prism.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SideEffectAttribute : Attribute
{
    public SideEffectAttribute(Type eventType)
    {
        EventType = eventType;
    }

    public Type EventType { get; }
    
    public string Key => $"{StreamTypeAttribute.GetStreamType(EventType)}:{EventType.Name}";
}