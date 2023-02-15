import '../styles/global.css'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "../lib/msal";
import {Alert} from "../components/Alert";
import {withApplicationInsights} from 'next-applicationinsights';

const MyApp = ({Component, pageProps}: AppProps) => {

    return <>
        <Head>
            <title>Pro Assistant</title>
        </Head>

        <MsalProvider instance={msalInstance}>

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
                <Alert/>
            </SWRConfig>

        </MsalProvider>
    </>
}

export default withApplicationInsights({
        namePrefix: 'proassistant',
        connectionString: process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING,
        isEnabled: true
    }
// @ts-ignore
)(MyApp);