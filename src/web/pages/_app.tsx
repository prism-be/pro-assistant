import '../styles/global.scss'

import type {AppProps} from 'next/app'
import {SWRConfig} from "swr";
import Head from "next/head";
import {getData} from "../lib/ajaxHelper";

const MyApp = ({Component, pageProps}: AppProps) => {

    return <>
        <Head>
            <title>Pro Assistant</title>
        </Head>
        <SWRConfig value={{fetcher: getData}}>
            <Component {...pageProps} />
        </SWRConfig>
    </>
}

export default MyApp;