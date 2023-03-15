import React from "react";
import Image, { ImageLoaderProps } from "next/image";

import useTranslation from "next-translate/useTranslation";

import { useUser } from "@auth0/nextjs-auth0/client";
import { toggledMobileMenu } from "@/modules/events/mobileMenu";
import Link from "next/link";
import * as process from "process";

const Header = () => {
    const { t } = useTranslation("common");

    const myLoader = ({ src }: ImageLoaderProps) => {
        return src;
    };

    const { user } = useUser();

    return (
        <>
            <div className={"h-14 flex border-b shadow"}>
                <div className={"p-1"} onClick={() => toggledMobileMenu()}>
                    <div>
                        <Image loader={myLoader} src="/images/logo.svg" height={42} width={42} alt={"ProAssistant by PRISM"} unoptimized={true}></Image>
                    </div>
                </div>
                <div className={"grow"}></div>
                <div className={"flex"}>
                    <div className="m-auto pl-2 pr-2 text-sm">
                        {t("header.hello")} {user?.name} !<br />
                        <a href={`/api/auth/logout?returnUrl=${encodeURIComponent(process.env.NEXT_PUBLIC_BASE_URL ?? "/")}`}>{t("header.logout")}</a>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Header;
