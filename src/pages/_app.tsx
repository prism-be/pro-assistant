import "@/styles/globals.css";

import type { AppProps } from "next/app";
import { SWRConfig } from "swr";
import Head from "next/head";
import { Alert } from "@/components/Alert";
import { getData } from "@/libs/http";
import { UserProvider } from "@auth0/nextjs-auth0/client";

const MyApp = ({ Component, pageProps }: AppProps) => {
  return (
    <UserProvider>
      <Head>
        <title>Pro Assistant</title>
      </Head>
      <SWRConfig value={{ fetcher: getData }}>
        <Component {...pageProps} />
        <Alert />
      </SWRConfig>
    </UserProvider>
  );
};

export default MyApp;
