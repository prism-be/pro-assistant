import styles from '../../styles/components/design/menu.module.scss';
import Link from "next/link";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";
import {useEffect, useState} from "react";
import {onToggledMobileMenu, toggledMobileMenu} from "../../lib/events/mobileMenu";

const Menu = () => {

    const {t} = useTranslation('common');
    const router = useRouter();
    const [displayMobileMenu, setDisplayMobileMenu]= useState(false);

    useEffect(() => {
        const subscription = onToggledMobileMenu().subscribe(() => {
            console.log(displayMobileMenu)
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
            <li className={styles.item + getActiveLinkClass('/patients')}><Link href={"/patients"}>{t("menu.patients")}</Link></li>
            <li className={styles.item + " " + styles.hideMobile + getActiveLinkClass('/calendar')}><Link href={"/calendar"}>{t("menu.calendar")}</Link></li>
            <li className={styles.title + " " + styles.hideMobile}>{t("menu.configurationTitle")}</li>
            <li className={styles.item + " " + styles.hideMobile + getActiveLinkClass('/configuration')}><Link href={"/configuration"}>{t("menu.configuration")}</Link></li>
            <li className={styles.item + " " + styles.hideMobile + getActiveLinkClass('/documents')}><Link href={"/documents"}>{t("menu.documents")}</Link></li>
        </ul>
    </div>
}

export default Menu;