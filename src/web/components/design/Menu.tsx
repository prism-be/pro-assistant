import styles from '../../styles/components/design/menu.module.scss';
import Link from "next/link";
import useTranslation from "next-translate/useTranslation";

const Menu = () => {

    const {t} = useTranslation('common');
    
    return <div className={styles.container}>
        <ul>
            <li className={styles.item}><Link href={"/patients"}><a>{t("menu.patients")}</a></Link></li>
            <li className={styles.item}><Link href={"/calendar"}><a>{t("menu.calendar")}</a></Link></li>
        </ul>
    </div>
}

export default Menu;