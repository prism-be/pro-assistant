import styles from '../styles/pages/calendar.module.scss';
import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import {useEffect, useState} from "react";
import {add, format, parseISO} from "date-fns";
import useTranslation from "next-translate/useTranslation";
import {getLocale} from "../lib/localization";
import React from 'react';
import {ArrowLeft, ArrowRight} from "../components/icons/Icons";
import {useKeyPressEvent} from "react-use";
import {getMeetings, Meeting} from "../lib/services/meetings";
import {popupNewMeeting} from "../lib/events/globalPopups";
import {onDataUpdated} from "../lib/events/data";

const Calendar: NextPage = () => {
    const getMonday = (d: Date) => {
        return add(d, {days: -d.getDay() + 1});
    }

    const [monday, setMonday] = useState(getMonday(new Date()));
    const [meetings, setMeetings] = useState<Meeting[]>([]);
    const {t} = useTranslation("common");
    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];
    const days = [1, 2, 3, 4, 5, 6, 7];

    useKeyPressEvent('ArrowLeft', () => {
        setMonday(add(monday, {weeks: -1}));
    })

    useKeyPressEvent('ArrowRight', () => {
        setMonday(add(monday, {weeks: 1}));
    })

    useEffect(() => {
        reloadMeetings();
        const subscription = onDataUpdated({type: "meeting"}).subscribe(() => {
            reloadMeetings();
        });
        return () => subscription.unsubscribe();
    }, [monday]);

    const reloadMeetings = async () => {
        const m = await getMeetings(monday, add(monday, {days: 8}));
        setMeetings(m);
    }

    const getDayClassName = (d: number) => {
        return styles["day" + d];
    }

    const getHourClassName = (h: number) => {
        return styles["hour" + h];
    }

    const getHourEndClassName = (h: number) => {
        return styles["hourEnd" + h];
    }

    const getDurationClassName = (d: number) => {
        d = Math.round(d / 30) * 30;
        return styles["duration" + d];
    }

    const addMeeting = (d: number, h: number, m: number) => {
        const startDate = add(new Date(monday.getFullYear(), monday.getMonth(), monday.getDate()), {days: d - 1, hours: h, minutes: m});
        popupNewMeeting({
            data: {
                startDate
            }
        });
    }


    return <ContentContainer>
        <>
            <h1>{t("pages.calendar.title")} {format(monday, "EEEE dd MMMM yyyy", {locale: getLocale()})}</h1>
            <div className={styles.navigationHeader}>
                <div className={styles.navigationLeft} onClick={() => setMonday(add(monday, {weeks: -1}))}><ArrowLeft/></div>
                <div className={styles.navigationRight} onClick={() => setMonday(add(monday, {weeks: 1}))}><ArrowRight/></div>
            </div>
            <div className={styles.calendar}>
                <div className={styles.hour + " " + styles.hour0}></div>

                {days.map(d => <div className={styles.headerDay + " " + getDayClassName(d)} key={d}>
                    {t("days.short.day" + d)} {format(add(monday, {days: d - 1}), "dd MMM", {locale: getLocale()})}
                </div>)}

                {hours.map(h => <div key={h} className={styles.hour + " " + getHourClassName(h)}>{h}H</div>)}

                {days.map(d => <React.Fragment key={d}>
                    {hours.map(h => <React.Fragment key={h}>
                        <div className={styles.dayAction + " " + styles.dayHourFirst + " " + getHourClassName(h) + " " + getDayClassName(d)} onClick={() => addMeeting(d, h, 0)}></div>
                        <div className={styles.dayAction + " " + styles.dayHourSecond + " " + getHourEndClassName(h) + " " + getDayClassName(d)} onClick={() => addMeeting(d, h, 30)}></div>
                    </React.Fragment>)}
                </React.Fragment>)}

                {meetings.map(m => <div className={styles.calendarItem + " " + getHourClassName(parseISO(m.startDate).getHours()) + " " + getDayClassName(parseISO(m.startDate).getDay()) + " " + getDurationClassName(m.duration)} key={m.id}
                                        onClick={() => {
                                            popupNewMeeting({data : { meetingId: m.id}});
                                        }}>
                    <div>{m.title?.slice(0, 30)}</div>
                </div>)}

            </div>
        </>
    </ContentContainer>
}

export default Calendar;