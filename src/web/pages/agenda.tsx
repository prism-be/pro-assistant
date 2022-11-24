﻿import styles from '../styles/pages/agenda.module.scss';

import {NextPage} from "next";
import Section from "../components/design/Section";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {useEffect, useState} from "react";
import {add, format, formatISO, parseISO, startOfToday} from "date-fns";
import {getLocale} from "../lib/localization";
import Button from "../components/forms/Button";
import {postData} from "../lib/ajaxHelper";
import {IMeeting} from "../lib/contracts";
import React from 'react';
import {popupNewMeeting} from "../lib/events/globalPopups";
import {onDataUpdated} from "../lib/events/data";
import Mobile from "../components/design/Mobile";
import {useSwipeable} from "react-swipeable";

const Agenda: NextPage = () => {

    const {t} = useTranslation("common");

    const [day, setDay] = useState(startOfToday());
    const [meetings, setMeetings] = useState<IMeeting[]>([]);
    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];

    async function reloadMeetings() {
        const m = await postData<IMeeting[]>("/meetings", {
            startDate: formatISO(day),
            endDate: formatISO(add(day, {days: 1}))
        });
        setMeetings(m ?? []);
    }

    useEffect(() => {
        reloadMeetings();
    }, [day]);

    useEffect(() => {
        const subscription = onDataUpdated({type: "meeting"}).subscribe(() => {
            reloadMeetings();
        });
        return () => subscription.unsubscribe();
    }, [day]);

    const swipeHandlers = useSwipeable({
        onSwipedLeft: () => setDay(add(day, {days: 1})),
        onSwipedRight: () => setDay(add(day, {days: -1})),
    });

    function getHourRowClassName(h: number) {
        return styles["hour" + h];
    }

    function getHourRowEndClassName(h: number) {
        return styles["hourEnd" + h];
    }

    function getDurationClassName(d: number) {
        d = Math.round(d / 30) * 30;
        return styles["duration" + d];
    }

    function addMeeting (h: number, m: number) {
        const startDate = add(new Date(day.getFullYear(), day.getMonth(), day.getDate()), {hours: h, minutes: m});
        popupNewMeeting({
            data: {
                startDate
            }
        });
    }
    return <ContentContainer>
        <Section>
            <>
                <div className={styles.agenda} {...swipeHandlers}>
                    <Button secondary={true} onClick={() => setDay(add(day, {days: -1}))} className={styles.previous} text={t("actions.prev")}></Button>
                    <Button secondary={true} onClick={() => setDay(add(day, {days: 1}))} className={styles.next} text={t("actions.nex")}></Button>

                    <Mobile className={styles.title} visible={false} breakpoint={"MD"}>
                        <h1>{t("pages.agenda.title")} {format(day, "EEEE d MMMM yyyy", {locale: getLocale()})}</h1>
                    </Mobile>

                    <Mobile className={styles.title} visible={true} breakpoint={"MD"}>
                        <h1>
                            {format(day, "EEEE", {locale: getLocale()})}
                            <br/>
                            {format(day, "d/MM/yy", {locale: getLocale()})}
                        </h1>
                    </Mobile>

                    {meetings.length === 0 && <div className={styles.noMeeting}>{t("pages.agenda.noMeeting")}</div>}

                    <div className={styles.gap}/>

                    {hours.map(h => <React.Fragment key={h}>
                        <div className={styles.hour + " " + getHourRowClassName(h)}>{h}H</div>
                        <div className={styles.halfHour + " " + getHourRowClassName(h)} onClick={() => addMeeting(h, 0)}></div>
                        <div className={styles.halfHourEnd + " " + getHourRowEndClassName(h)} onClick={() => addMeeting(h, 30)}></div>
                    </React.Fragment>)}

                    {meetings.map(m =>
                        <div className={styles.calendarItem + " " + getHourRowClassName(parseISO(m.startDate).getHours()) + " " + getDurationClassName(m.duration)} key={m.id}
                             onClick={() => {
                                 window.scroll({top: 0});
                                 popupNewMeeting({data: {meetingId: m.id}});
                             }}>
                            <div>{m.title?.slice(0, 40)}</div>
                        </div>)}

                </div>
            </>
        </Section>
    </ContentContainer>
}

export default Agenda;