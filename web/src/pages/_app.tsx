import "@/styles/globals.css";

import type {AppProps} from "next/app";
import {SWRConfig} from "swr";
import Head from "next/head";
import {Alert} from "@/components/Alert";
import {getData} from "@/libs/http";
import Insights from "@/components/Insights";
import {MsalAuthenticationTemplate, MsalProvider} from "@azure/msal-react";
import {msalInstance} from "@/libs/msal";
import {defaultLanguage, defaultNamespace, I18nProvider, languages, namespaces,} from "next-i18next-static-site";
import {enableReactUse} from '@legendapp/state/config/enableReactUse';
import { enableReactComponents } from '@legendapp/state/config/enableReactComponents'

import locales from "@/libs/localization";
import {InteractionType} from "@azure/msal-browser";

enableReactUse();
enableReactComponents();

const MyApp = ({Component, pageProps}: AppProps) => {

    const i18n = {
        languages,
        defaultLanguage,
        namespaces,
        defaultNamespace,
        locales,
    };

    const authRequest = {
        scopes: ["openid", "profile"]
    };

    return (
        <Insights>
            <MsalProvider instance={msalInstance}>
                <MsalAuthenticationTemplate
                    interactionType={InteractionType.Redirect}
                    authenticationRequest={authRequest}
                >
                    <Head>
                        <title>Pro Assistant</title>
                    </Head>
                    <SWRConfig value={{fetcher: getData}}>
                        <I18nProvider i18n={i18n}>
                            <Component {...pageProps} />
                        </I18nProvider>
                        <Alert/>
                    </SWRConfig>
                </MsalAuthenticationTemplate>
            </MsalProvider>
        </Insights>
    );
};

export default MyApp;
