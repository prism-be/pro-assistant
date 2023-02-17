import Link from "next/link";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";
import {useEffect, useState} from "react";
import {onToggledMobileMenu, toggledMobileMenu} from "../../lib/events/mobileMenu";
import {format} from "date-fns";

const Menu = () => {

    const {t} = useTranslation('common');
    const router = useRouter();
    const [displayMobileMenu, setDisplayMobileMenu] = useState(false);

    useEffect(() => {
        const subscription = onToggledMobileMenu().subscribe(() => {
            setDisplayMobileMenu(!displayMobileMenu);
        });
        return () => subscription.unsubscribe();
    }, [displayMobileMenu]);

    const getActiveLinkClass = (path: string) => {
        if (router.pathname.startsWith(path)) {
            return " border-r-4 border-primary bg-gray-100 ";
        }

        return "";
    }

    return <div className={"bg-white w-64 border-r h-100 fixed top-14 h-full " + (displayMobileMenu ? "block" : "hidden md:block")} onClick={() => toggledMobileMenu()}>
        <ul>
            <li className={"px-4 pt-4 font-bold"}>{t("menu.global")}</li>
            <li className={getActiveLinkClass('/contacts')}><Link className={"block px-4 py-2"} href={"/contacts"}>{t("menu.contacts")}</Link></li>
            <li className={getActiveLinkClass('/agenda')}><Link className={"block px-4 py-2"} href={"/agenda/" + format(new Date(), "yyyy-MM-dd")}>{t("menu.agenda")}</Link></li>
            <li className={getActiveLinkClass('/calendar')}><Link className={"block px-4 py-2"} href={"/calendar"}>{t("menu.calendar")}</Link></li>
            <li className={getActiveLinkClass('/appointments')}><Link className={"block px-4 py-2"} href={"/appointments"}>{t("menu.appointments")}</Link></li>

            <div className={"hidden md:block"}>
                <li className={"px-4 pt-4 font-bold"}>{t("menu.configurationTitle")}</li>
                <li className={getActiveLinkClass('/configuration')}><Link className={"block px-4 py-2"} href={"/configuration"}>{t("menu.configuration")}</Link></li>
                <li className={getActiveLinkClass('/documents')}><Link className={"block px-4 py-2"} href={"/documents"}>{t("menu.documents")}</Link></li>
            </div>

            <li className={"px-4 pt-4 font-bold"}>{t("menu.accounting")}</li>
            <li className={getActiveLinkClass('/accounting/closing')}><Link className={"block px-4 py-2"} href={"/accounting/closing"}>{t("menu.accounting-closing")}</Link></li>
        </ul>
    </div>
}

export default Menu;