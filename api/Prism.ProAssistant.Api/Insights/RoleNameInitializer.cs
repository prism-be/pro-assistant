using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Prism.ProAssistant.Api.Helpers;

namespace Prism.ProAssistant.Api.Insights;

public class RoleNameInitializer : ITelemetryInitializer
{

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = EnvironmentConfiguration.GetMandatoryConfiguration("ENVIRONMENT");
    }
}