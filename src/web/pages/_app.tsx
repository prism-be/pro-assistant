import '../styles/global.scss'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "../lib/msal";
import {Alert} from "../components/Alert";
import {GlobalPopupsContainer} from "../components/global-popups/GlobalPopupsContainer";
import { withApplicationInsights } from 'next-applicationinsights';
import getConfig from "next/config";

const { publicRuntimeConfig: config } = getConfig()

const MyApp = ({Component, pageProps}: AppProps) => {

    return <>
        <Head>
            <title>Pro Assistant</title>
        </Head>

        <MsalProvider instance={msalInstance}>

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
                <GlobalPopupsContainer />
                <Alert />
            </SWRConfig>

        </MsalProvider>
    </>
}

export default withApplicationInsights({
    namePrefix: 'proassistant',
    connectionString: config.aiConnectionString,
    isEnabled: true}
// @ts-ignore
)(MyApp);