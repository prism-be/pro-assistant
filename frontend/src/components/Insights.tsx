﻿import {ApplicationInsights} from "@microsoft/applicationinsights-web";
import {AppInsightsContext, ReactPlugin, withAITracking} from "@microsoft/applicationinsights-react-js";

let reactPlugin = new ReactPlugin();
let appInsights = new ApplicationInsights({
    config: {
        connectionString: import.meta.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING,
        enableAutoRouteTracking: true,
        enableCorsCorrelation: true,
        isBrowserLinkTrackingEnabled: true,
        extensions: [reactPlugin],
    },
});

try {
    appInsights.loadAppInsights();
} catch (e) {}

interface Props {
    children: any;
}

const AzureAppInsights = ({ children }: Props) => {
    return <AppInsightsContext.Provider value={reactPlugin}>{children}</AppInsightsContext.Provider>;
};

export default withAITracking(reactPlugin, AzureAppInsights);
