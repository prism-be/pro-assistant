import styles from '../../styles/modules/header.module.scss';

import React from "react";
import Image, {ImageLoaderProps} from "next/image";

import useTranslation from "next-translate/useTranslation"
import {useRouter} from "next/router";
import {useSWRConfig} from "swr";

import Link from "next/link";

const Header = () => {

    const {t} = useTranslation('common');

    const router = useRouter();
    const {mutate} = useSWRConfig();

    const logout = async (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault();
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        await mutate('/api/authentication/user/check');
        await router.push('/login');
    }

    const myLoader = ({src}: ImageLoaderProps) => {
        return src;
    }

    return <>
        <div className={styles.bar}>
            <div className={styles.logo}>
                <Link href="/">
                    <a>
                        <Image loader={myLoader} src="/images/logo.svg" height={42} width={42} alt={"ProAssistant by PRISM"} unoptimized={true}></Image>
                    </a>
                </Link>
            </div>
            <div className={styles.menu}>
            </div>
            <div className={styles.hello}>
                <div className="m-auto pl-2 pr-2 text-sm">
                    {t("header.hello")} {"Simon"} !<br/>
                    <a href="#" onClick={logout}>{t("header.logout")}</a>

                </div>
            </div>
        </div>
    </>
}

export default Header;