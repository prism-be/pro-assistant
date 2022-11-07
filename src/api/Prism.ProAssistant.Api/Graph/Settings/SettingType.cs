// -----------------------------------------------------------------------
//  <copyright file = "MeetingType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Settings;

public class SettingType : ObjectType<Setting>
{
    public SettingType() : base(ConfigureSetting)
    {
    }

    internal static void ConfigureSetting(IObjectTypeDescriptor<Setting> descriptor)
    {
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.Value);
    }
}