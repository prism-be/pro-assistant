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
            <li className={styles.item + getActiveLinkClass('/patients')}><Link href={"/patients"}><a>{t("menu.patients")}</a></Link></li>
            <li className={styles.item + getActiveLinkClass('/calendar')}><Link href={"/calendar"}><a>{t("menu.calendar")}</a></Link></li>
            <li className={styles.item + getActiveLinkClass('/configuration')}><Link href={"/configuration"}><a>{t("menu.configuration")}</a></Link></li>
        </ul>
    </div>
}

export default Menu;