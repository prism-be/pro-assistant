import styles from '../styles/pages/agenda.module.scss';

import {NextPage} from "next";
import Section from "../components/design/Section";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {useState} from "react";
import {add, format, getDate} from "date-fns";
import {getLocale} from "../lib/localization";
import Button from "../components/forms/Button";

const Agenda: NextPage = () => {
    
    const {t} = useTranslation("common");

    const [day, setDay] = useState(new Date());
    
    return <ContentContainer>
        <Section>
        <>
            <div className={styles.agenda}>
                <Button onClick={() => setDay(add(day, {days: -1}))} className={styles.previous} text={t("actions.previous")}></Button>
                <Button onClick={() => setDay(add(day, {days: 1}))} className={styles.next} text={t("actions.next")}></Button>
            </div>
            <h1 className={styles.title}>{t("pages.agenda.title")} {format(day, "EEEE d MMMM yyyy", {locale: getLocale()})}</h1>
        </>
    </Section>
    </ContentContainer>
}

export default Agenda;