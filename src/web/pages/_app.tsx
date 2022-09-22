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
            domain={process.env.NEXT_PUBLIC_AUTH0_DOMAIN!}
            clientId={process.env.NEXT_PUBLIC_AUTH0_CLIENT_ID!}
            redirectUri={process.env.NEXT_PUBLIC_AUTH0_REDIRECT_URI!}
            audience={process.env.NEXT_PUBLIC_AUTH0_AUDIENCE!}
            scope={process.env.NEXT_PUBLIC_AUTH0_REDIRECT_URI!}
            useRefreshTokens={true}
        >

            <SWRConfig value={{fetcher: getData}}>
                <Component {...pageProps} />
            </SWRConfig>

        </Auth0Provider>
    </>
}

export default MyApp;