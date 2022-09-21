import {Html, Head, Main, NextScript} from 'next/document'
import i18nextConfig from '../next-i18next.config'

export default function Document(this: any) {

    const currentLocale = i18nextConfig.i18n.defaultLocale
    
    // noinspection HtmlRequiredTitleElement
    return (
        <Html lang={currentLocale}>
            <Head>
                <link rel="preconnect" href="https://fonts.googleapis.com"/>
                <link rel="preconnect" href="https://fonts.gstatic.com"/>
                <link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,400;0,700;1,400;1,700&display=swap" rel="stylesheet"/>
                <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet"/>
                <meta name="apple-mobile-web-app-capable" content="yes" />
                <link rel="icon" type="image/png" href="/favicon.png"/>
            </Head>
            <body>
            <Main/>
            <NextScript/>
            </body>
        </Html>
    )
}