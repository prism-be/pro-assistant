import styles from '../styles/pages/calendar.module.scss';
import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import {useState} from "react";
import {add, format, formatISO} from "date-fns";
import useTranslation from "next-translate/useTranslation";
import {getLocale} from "../lib/localization";
import React from 'react';
import {ArrowLeft, ArrowRight} from "../components/icons/Icons";
import {useKeyPressEvent} from "react-use";

const Calendar: NextPage = () => {
    const getMonday = (d: Date) => {
        return add(d, {days: -d.getDay() + 1});
    }

    const [monday, setMonday] = useState(getMonday(new Date()))
    const {t} = useTranslation("common");
    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];
    const days = [1, 2, 3, 4, 5, 6, 7];

    useKeyPressEvent('ArrowLeft', () => {
        setMonday(add(monday, { weeks: -1}));
    })

    useKeyPressEvent('ArrowRight', () => {
        setMonday(add(monday, { weeks: 1}));
    })

    return <ContentContainer>
        <>
            <h1>{t("pages.calendar.title")} {format(monday, "EEEE dd MMMM yyyy", {locale: getLocale()})}</h1>
            <div className={styles.navigationHeader}>
                <div className={styles.navigationLeft} onClick={() => setMonday(add(monday, { weeks: -1}))}><ArrowLeft /></div>
                <div className={styles.navigationRight} onClick={() => setMonday(add(monday, { weeks: 1}))}><ArrowRight /></div>
            </div>
            <div className={styles.calendar}>
                <div className={styles.hour} style={{gridRowStart: 1}}></div>
                {days.map(d => <div className={styles.headerDay} key={d} style={{gridColumnStart: d + 1}}>
                    {t("days.short.day" + d)} {format(add(monday, {days: d - 1}), "dd MMM", {locale: getLocale()})}
                </div>)}
                {hours.map(h => <div key={h} className={styles.hour} style={{gridRowStart: h * 2}}>{h}H</div>)}
                {days.map(d => <React.Fragment key={d}>
                    {hours.map(h => <React.Fragment key={h}>
                        <div className={styles.dayHourFirst} style={{gridRowStart: h * 2, gridColumnStart: d + 1}}></div>
                        <div className={styles.dayHourSecond} style={{gridRowStart: h * 2 + 1, gridColumnStart: d + 1}}></div>
                    </React.Fragment>)}
                </React.Fragment>)}
            </div>
        </>
    </ContentContainer>
}

export default Calendar;