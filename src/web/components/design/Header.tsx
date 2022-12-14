import styles from '../../styles/components/design/header.module.scss';

import React from "react";
import Image, {ImageLoaderProps} from "next/image";

import useTranslation from "next-translate/useTranslation"
import useSWR from "swr";

import {useMsal} from "@azure/msal-react";
import {getData} from "../../lib/ajaxHelper";
import {toggledMobileMenu} from "../../lib/events/mobileMenu";
import {UserInformation} from "../../lib/contracts";
import Head from "next/head";

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
        <div className={styles.bar}>
            <div className={styles.logo} onClick={() => toggledMobileMenu()}>
                <div>
                    <Image loader={myLoader} src="/images/logo.svg" height={42} width={42} alt={"ProAssistant by PRISM"} unoptimized={true}></Image>
                </div>
            </div>
            <div className={styles.menu}>
            </div>
            <div className={styles.hello}>
                <div className="m-auto pl-2 pr-2 text-sm">
                    {t("header.hello")} {data?.name} !<br/>
                    <a href="#" onClick={() => instance.logout()}>{t("header.logout")}</a>
                </div>
            </div>
        </div>
    </>
}

export default Header;