import styles from '../../styles/components/design/menu.module.scss';
import Link from "next/link";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";
import {useEffect, useState} from "react";
import {onToggledMobileMenu, toggledMobileMenu} from "../../lib/events/mobileMenu";
import Mobile from "./Mobile";
import {format} from "date-fns";

const Menu = () => {

    const {t} = useTranslation('common');
    const router = useRouter();
    const [displayMobileMenu, setDisplayMobileMenu]= useState(false);

    useEffect(() => {
        const subscription = onToggledMobileMenu().subscribe(() => {
            setDisplayMobileMenu(!displayMobileMenu);
        });
        return () => subscription.unsubscribe();
    }, [displayMobileMenu]);
    
    const getActiveLinkClass = (path: string) =>
    {
        if (router.pathname.startsWith(path))
        {
            return " " + styles.active;
        }
        
        return "";
    }
    
    return <div className={styles.container + (displayMobileMenu ? " " + styles.mobile : "")}  onClick={() => toggledMobileMenu()}>
        <ul>
            <li className={styles.title}>{t("menu.global")}</li>
            <li className={styles.item + getActiveLinkClass('/contacts')}><Link href={"/contacts"}>{t("menu.contacts")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/agenda')}><Link href={"/agenda/" + format(new Date(), "yyyy-MM-dd")}>{t("menu.agenda")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/calendar')}><Link href={"/calendar"}>{t("menu.calendar")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/appointments')}><Link href={"/appointments"}>{t("menu.appointments")}</Link></li>
            
            <Mobile breakpoint={"MD"}>
                <li className={styles.title}>{t("menu.configurationTitle")}</li>
                <li className={styles.item + getActiveLinkClass('/configuration')}><Link href={"/configuration"}>{t("menu.configuration")}</Link></li>
                <li className={styles.item + getActiveLinkClass('/documents')}><Link href={"/documents"}>{t("menu.documents")}</Link></li>
            </Mobile>
        </ul>
    </div>
}

export default Menu;