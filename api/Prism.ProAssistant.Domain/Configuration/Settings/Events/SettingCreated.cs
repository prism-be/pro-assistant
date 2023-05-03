namespace Prism.ProAssistant.Domain.Configuration.Settings.Events;

using Core.Attributes;

[StreamType(Streams.Settings)]
public class SettingCreated: BaseEvent
{
    required public Setting Setting { get; set; }
    public override string StreamId => Setting.Id;
}