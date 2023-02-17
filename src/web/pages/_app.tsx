import '../styles/global.css'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "../lib/msal";
import {Alert} from "../components/Alert";
import Insights from "../components/Insights";

const MyApp = ({Component, pageProps}: AppProps) => {

    return <Insights>
        <Head>
            <title>Pro Assistant</title>
        </Head>


        <MsalProvider instance={msalInstance}>

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
                <Alert/>
            </SWRConfig>

        </MsalProvider>
    </Insights>
}

export default MyApp;