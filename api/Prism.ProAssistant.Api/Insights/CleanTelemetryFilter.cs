using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Prism.ProAssistant.Api.Insights;

public class CleanTelemetryFilter: ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public CleanTelemetryFilter(ITelemetryProcessor next)
    {
        _next = next;
    }
    
    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry request)
        {
            if (request.Url.LocalPath.StartsWith("/health", StringComparison.InvariantCultureIgnoreCase))
            {
                // Do not send telemetry data for health checks
                return;
            }
        }
        
        // Process the telemetry item with the next processor in the chain
        _next.Process(item);
    }
}