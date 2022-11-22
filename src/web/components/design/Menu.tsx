import styles from '../../styles/components/design/menu.module.scss';
import Link from "next/link";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";

const Menu = () => {

    const {t} = useTranslation('common');
    const router = useRouter();
    
    const getActiveLinkClass = (path: string) =>
    {
        if (router.pathname.startsWith(path))
        {
            return " " + styles.active;
        }
        
        return "";
    }
    
    return <div className={styles.container}>
        <ul>
            <li className={styles.item + getActiveLinkClass('/patients')}><Link href={"/patients"}>{t("menu.patients")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/calendar')}><Link href={"/calendar"}>{t("menu.calendar")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/configuration')}><Link href={"/configuration"}>{t("menu.configuration")}</Link></li>
            <li className={styles.item + getActiveLinkClass('/documents')}><Link href={"/documents"}>{t("menu.documents")}</Link></li>
        </ul>
    </div>
}

export default Menu;