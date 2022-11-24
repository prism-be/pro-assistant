// -----------------------------------------------------------------------
//  <copyright file = "RoleNameInitializer.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Prism.ProAssistant.Business;

namespace Prism.ProAssistant.Api.Insights;

public class RoleNameInitializer: ITelemetryInitializer
{

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = EnvironmentConfiguration.GetMandatoryConfiguration("ENVIRONMENT");
    }
}