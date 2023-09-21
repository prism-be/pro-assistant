import React from "react";
import Image, {ImageLoaderProps} from "next/image";

import {useTranslation} from "react-i18next";

import {useMsal} from "@azure/msal-react";
import Head from "next/head";
import {getData} from "@/libs/http";
import {UserInformation} from "@/libs/models";
import {toggledMobileMenu} from "@/libs/events/mobileMenu";
import {useObserveEffect} from "@legendapp/state/react";
import {currentUser$} from "@/stores/user";

const Header = () => {

    const {t} = useTranslation('common');

    const myLoader = ({src}: ImageLoaderProps) => {
        return src;
    }

    const {instance} = useMsal();

    useObserveEffect(async () => {
        const user = await getData<UserInformation>("/authentication/user");
        currentUser$.set(user);
    });

    return <>
        <Head>
            <title>Pro Assistant - {currentUser$.get()?.organization}</title>
        </Head>
        <div className={"h-14 flex border-b shadow print:hidden"}>
            <div className={"p-1"} onClick={() => toggledMobileMenu()}>
                <div>
                    <Image loader={myLoader} src="/images/logo.svg" height={42} width={42} alt={"ProAssistant by PRISM"}
                           unoptimized={true}></Image>
                </div>
            </div>
            <div className={"grow"}>
            </div>
            <div className={"flex"}>
                <div className="m-auto pl-2 pr-2 text-sm">
                    {t("header.hello")} {currentUser$.get()?.name} !<br/>
                    <a href="#" onClick={() => instance.logout()}>{t("header.logout")}</a>
                </div>
            </div>
        </div>
    </>
}

export default Header;