import "@/styles/globals.css";

import type {AppProps} from "next/app";
import {SWRConfig} from "swr";
import Head from "next/head";
import {Alert} from "@/components/Alert";
import {getData} from "@/libs/http";
import Insights from "@/components/Insights";
import {MsalProvider} from "@azure/msal-react";
import {msalInstance} from "@/libs/msal";
import {I18nProvider, languages, defaultLanguage, namespaces, defaultNamespace,} from "next-i18next-static-site";

import locales from "@/libs/localization";

const MyApp = ({Component, pageProps}: AppProps) => {

    const i18n = {
        languages,
        defaultLanguage,
        namespaces,
        defaultNamespace,
        locales,
    };

    return (
            <Insights>
                <MsalProvider instance={msalInstance}>
                    <Head>
                        <title>Pro Assistant</title>
                    </Head>
                    <SWRConfig value={{fetcher: getData}}>
                        <I18nProvider i18n={i18n}>
                            <Component {...pageProps} />
                        </I18nProvider>
                        <Alert/>
                    </SWRConfig>
                </MsalProvider>
            </Insights>
    );
};

export default MyApp;
