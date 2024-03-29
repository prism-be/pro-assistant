﻿using Prism.ProAssistant.Domain.Configuration.Settings.Events;

namespace Prism.ProAssistant.Domain.Configuration.Settings;

public class SettingAggregator : IDomainAggregator<Setting>
{

    public void Init(string id)
    {
        State = new Setting { Id = id };
    }

    public Setting? State { get; private set; }

    public Task When(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case nameof(SettingCreated):
                Apply(@event.ToEvent<SettingCreated>());
                break;
            case nameof(SettingUpdated):
                Apply(@event.ToEvent<SettingUpdated>());
                break;
        }
        
        return Task.CompletedTask;
    }

    public Task Complete()
    {
        return Task.CompletedTask;
    }

    private void Apply(SettingCreated @event)
    {
        State = @event.Setting;
    }
    
    private void Apply(SettingUpdated @event)
    {
        State = @event.Setting;
    }
}