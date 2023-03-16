import React from "react";
import Image, {ImageLoaderProps} from "next/image";

import useTranslation from "next-translate/useTranslation"
import useSWR from "swr";

import {useMsal} from "@azure/msal-react";
import Head from "next/head";
import {getData} from "@/libs/http";
import {UserInformation} from "@/libs/models";
import {toggledMobileMenu} from "@/modules/events/mobileMenu";

const Header = () => {

    const {t} = useTranslation('common');

    const myLoader = ({src}: ImageLoaderProps) => {
        return src;
    }

    const {instance} = useMsal();
    const {data} = useSWR('/authentication/user', (apiURL: string) => getData<UserInformation>(apiURL))

    return <>
        <Head>
            <title>Pro Assistant - {data?.organization}</title>
        </Head>
        <div className={"h-14 flex border-b shadow"}>
            <div className={"p-1"} onClick={() => toggledMobileMenu()}>
                <div>
                    <Image loader={myLoader} src="/images/logo.svg" height={42} width={42} alt={"ProAssistant by PRISM"} unoptimized={true}></Image>
                </div>
            </div>
            <div className={"grow"}>
            </div>
            <div className={"flex"}>
                <div className="m-auto pl-2 pr-2 text-sm">
                    {t("header.hello")} {data?.name} !<br/>
                    <a href="#" onClick={() => instance.logout()}>{t("header.logout")}</a>
                </div>
            </div>
        </div>
    </>
}

export default Header;