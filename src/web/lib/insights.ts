import {ApplicationInsights} from '@microsoft/applicationinsights-web';
import {ReactPlugin} from '@microsoft/applicationinsights-react-js';

const defaultBrowserHistory =
    {
        listen: () => {
        },
        location: {
            pathname: '',
            search: '',
        },
        url: '',
    };

let browserHistory = defaultBrowserHistory;

if (typeof window !== 'undefined') {
    browserHistory = {...browserHistory, ...window.history};
    browserHistory.location.pathname = (browserHistory as any)?.state?.url;
}

const reactPlugin = new ReactPlugin();
const appInsights = new ApplicationInsights({
    config: {
        connectionString: process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING,
        extensions: [reactPlugin],
        extensionConfig: {
            [reactPlugin.identifier]: {history: browserHistory}
        }
    }
});

if (typeof window !== 'undefined' && process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING !== "") {
    appInsights.loadAppInsights();
}

export {reactPlugin, appInsights};