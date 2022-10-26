import {add, format, formatISO} from 'date-fns';
import styles from '../../styles/components/forms/calendar.module.scss'
import {useEffect, useState} from "react";
import {ArrowLeft, ArrowRight} from "../icons/Icons";
import { fr } from 'date-fns/locale'
import useTranslation from "next-translate/useTranslation";

interface Props {
    value?: Date;
    onChange: (date: Date) => void;
    className?: string;
}

export const Calendar = ({value, onChange, className}: Props) => {

    const current = value ?? new Date();
    const {t} = useTranslation('common');
    const [month, setMonth] = useState<Date>(new Date(current.getFullYear(), current.getMonth()));
    const [dates, setDates]= useState<Date[]>([]);
    
    useEffect(() => {
        let c = month;
        let d: Date[] = [];
        
        while (c.getMonth() === month.getMonth())
        {
            d.push(new Date(c));
            c = add(c, {days: 1});
        }
        
        setDates(d);
    }, [month]);

    const getClass = (d: Date) => {
        let css = "";
        switch (d.getDay())
        {
            case 0:
                css = styles.day7;
                break;
            case 1:
                css = styles.day1;
                break;
            case 2:
                css = styles.day2;
                break;
            case 3:
                css = styles.day3;
                break;
            case 4:
                css = styles.day4;
                break;
            case 5:
                css = styles.day5;
                break;
            case 6:
                css = styles.day6;
                break;
        }
        
        if (d.getFullYear() === current.getFullYear() && d.getMonth() === current.getMonth() && d.getDate() === current.getDate())
        {
            css += " " + styles.daySelected;
        }
        
        return css;
    }
    
    return <div className={styles.calendar + " " + className}>
        <div className={styles.previous} onClick={() => setMonth(add(month, { months: -1 })) }><ArrowLeft /></div>
        <div className={styles.next} onClick={() => setMonth(add(month, { months: 1 })) }><ArrowRight /></div>
        <div className={styles.month}>{format(month, "MMMM yyyy", { locale: fr })}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day1}>{t("days.short.day1")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day2}>{t("days.short.day2")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day3}>{t("days.short.day3")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day4}>{t("days.short.day4")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day5}>{t("days.short.day5")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day6}>{t("days.short.day6")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day7}>{t("days.short.day7")}</div>
        {dates.map(d => <div key={formatISO(d)} className={styles.day + " " + getClass(d)} onClick={() => onChange(d)}>{d.getDate()}</div>)}
    </div>
}