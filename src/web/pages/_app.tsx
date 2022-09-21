import '../styles/global.scss'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";
import {Auth0Provider} from "@auth0/auth0-react";

const MyApp = ({Component, pageProps}: AppProps) => {

    return <>
        <Head>
            <title>Pro Assistant</title>
        </Head>

        <Auth0Provider
            domain="by-prism.eu.auth0.com"
            clientId="Okfgjry7fkHqX2l0A4cQXu6jLXqLt897"
            redirectUri="http://localhost:3000"
            audience="https://localhost:7013"
            scope="read:current_user update:current_user_metadata"
            useRefreshTokens={true}
        >

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
            </SWRConfig>

        </Auth0Provider>
    </>
}

export default MyApp;