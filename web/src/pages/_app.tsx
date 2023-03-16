import "@/styles/globals.css";

import type {AppProps} from "next/app";
import {SWRConfig} from "swr";
import Head from "next/head";
import {Alert} from "@/components/Alert";
import {getData} from "@/libs/http";
import Insights from "@/components/Insights";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "@/libs/msal";

const MyApp = ({Component, pageProps}: AppProps) => {
    return (
            <Insights>
                <MsalProvider instance={msalInstance}>
                    <Head>
                        <title>Pro Assistant</title>
                    </Head>
                    <SWRConfig value={{fetcher: getData}}>
                        <Component {...pageProps} />
                        <Alert/>
                    </SWRConfig>
                </MsalProvider>
            </Insights>
    );
};

export default MyApp;
