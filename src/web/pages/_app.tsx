import '../styles/global.scss'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "../lib/msal";

const MyApp = ({Component, pageProps}: AppProps) => {

    return <>
        <Head>
            <title>Pro Assistant</title>
        </Head>

        <MsalProvider instance={msalInstance}>

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
            </SWRConfig>

        </MsalProvider>
    </>
}

export default MyApp;