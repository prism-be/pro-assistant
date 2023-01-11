import {add, format, formatISO, startOfMonth} from 'date-fns';
import styles from '../../styles/components/forms/calendar.module.scss'
import {useEffect, useState} from "react";
import useTranslation from "next-translate/useTranslation";
import {getLocale} from "../../lib/localization";
import { ArrowSmallLeftIcon, ArrowSmallRightIcon } from '@heroicons/react/24/outline';

interface Props {
    value: Date;
    onChange: (date: Date) => void;
    className?: string;
}

export const Calendar = ({value, onChange, className}: Props) => {

    const {t} = useTranslation('common');
    const [date, setDate] = useState<Date>(value);
    const [month, setMonth] = useState<Date>(startOfMonth(value));
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
    
    useEffect(() => {
        setDate(value);
        setMonth(startOfMonth(value));
    }, [value]);

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
        
        if (d.getFullYear() === date.getFullYear() && d.getMonth() === date.getMonth() && d.getDate() === date.getDate())
        {
            css += " " + styles.daySelected;
        }
        
        return css;
    }
    
    return <div className={styles.calendar + " " + className} title={formatISO(value)}>
        <div className={styles.previous} onClick={() => setMonth(add(month, { months: -1 })) }><ArrowSmallLeftIcon /></div>
        <div className={styles.next} onClick={() => setMonth(add(month, { months: 1 })) }><ArrowSmallRightIcon /></div>
        <div className={styles.month}>{format(month, "MMMM yyyy", { locale: getLocale() })}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day1}>{t("days.short.day1")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day2}>{t("days.short.day2")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day3}>{t("days.short.day3")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day4}>{t("days.short.day4")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day5}>{t("days.short.day5")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day6}>{t("days.short.day6")}</div>
        <div className={styles.dayHeader + " " + styles.day + " " + styles.day7}>{t("days.short.day7")}</div>
        {dates.map(d => <div key={formatISO(d)} className={styles.day + " " + getClass(d)} onClick={() => {
            onChange(d);
            setDate(d);
        }}>{d.getDate()}</div>)}
    </div>
}