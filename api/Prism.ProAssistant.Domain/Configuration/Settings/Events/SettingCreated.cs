namespace Prism.ProAssistant.Domain.Configuration.Settings.Events;

public class SettingCreated: IDomainEvent
{
    required public Setting Setting { get; set; }
    public string StreamId => Setting.Id;
    public string StreamType => "settings";
}