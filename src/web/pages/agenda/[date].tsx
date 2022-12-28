import styles from '../../styles/pages/agenda.module.scss';

import {NextPage} from "next";
import Section from "../../components/design/Section";
import ContentContainer from "../../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {add, format, formatISO, parse, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import Button from "../../components/forms/Button";
import {postData} from "../../lib/ajaxHelper";
import {Meeting} from "../../lib/contracts";
import React from 'react';
import Mobile from "../../components/design/Mobile";
import {useSwipeable} from "react-swipeable";
import {useRouter} from "next/router";
import useSWR from "swr";

const Agenda: NextPage = () => {
    const router = useRouter();
    const day = parse(router.query.date as string, "yyyy-MM-dd", new Date());
    const {t} = useTranslation("common");

    const {data: meetings} = useSWR(router.asPath, loadMeetings);

    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];

    async function loadMeetings() {
        return await postData<Meeting[]>("/meetings", {
            startDate: formatISO(day),
            endDate: formatISO(add(day, {days: 1}))
        });
    }

    function changeDay(delta: number) {
        const newDay = add(day, {days: delta});
        router.push("/agenda/" + format(newDay, "yyyy-MM-dd"));
    }

    const swipeHandlers = useSwipeable({
        onSwipedLeft: () => changeDay(1),
        onSwipedRight: () => changeDay(-1),
    });

    function getHourRowClassName(h: number, m: number) {
        if (m < 30) {
            return styles["hour" + h];
        }
        
        return styles["hourEnd" + h];
    }

    function getHourRowEndClassName(h: number) {
        return styles["hourEnd" + h];
    }

    function getDurationClassName(d: number) {
        d = Math.round(d / 30) * 30;
        return styles["duration" + d];
    }

    function addMeeting(h: number, m: number) {
        const startDate = add(new Date(day.getFullYear(), day.getMonth(), day.getDate()), {hours: h, minutes: m});
        router.push("/meetings/new?startDate=" + encodeURIComponent(formatISO(startDate)));
    }
    
    function previousDay() {
        changeDay(-1);
    }
    
    function nextDay() { 
        changeDay(1);
    }

    return <ContentContainer>
        <Section>
            <>
                <div className={styles.agenda} {...swipeHandlers}>
                    <Button secondary={true} onClick={previousDay} className={styles.previous} text={t("actions.prev")}></Button>
                    <Button secondary={true} onClick={nextDay} className={styles.next} text={t("actions.nex")}></Button>

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

                    {meetings?.length === 0 && <div className={styles.noMeeting}>{t("pages.agenda.noMeeting")}</div>}

                    <div className={styles.gap}/>

                    {hours.map(h => <React.Fragment key={h}>
                        <div className={styles.hour + " " + getHourRowClassName(h, 0)}>{h}H</div>
                        <div className={styles.halfHour + " " + getHourRowClassName(h, 0)} onClick={() => addMeeting(h, 0)}></div>
                        <div className={styles.halfHourEnd + " " + getHourRowEndClassName(h)} onClick={() => addMeeting(h, 30)}></div>
                    </React.Fragment>)}

                    {meetings?.map(m =>
                        <div className={styles.calendarItem + " " + getHourRowClassName(parseISO(m.startDate).getHours(), parseISO(m.startDate).getMinutes()) + " " + getDurationClassName(m.duration)} key={m.id}
                             onClick={() => router.push("/meetings/" + m.id)}
                             style={{backgroundColor: m.backgroundColor ?? ""}}>
                            <div>{m.title?.slice(0, 40)}</div>
                        </div>)}

                </div>
            </>
        </Section>
    </ContentContainer>
}

export default Agenda;