import styles from '../../styles/pages/calendar.module.scss';
import {NextPage} from "next";
import ContentContainer from "../../components/design/ContentContainer";
import {add, format, formatISO, parse, parseISO, startOfWeek} from "date-fns";
import useTranslation from "next-translate/useTranslation";
import {getLocale} from "../../lib/localization";
import React from 'react';
import {useKeyPressEvent} from "react-use";
import {Appointment} from "../../lib/contracts";
import {postData} from "../../lib/ajaxHelper";
import Mobile from "../../components/design/Mobile";
import {useSwipeable} from "react-swipeable";
import Section from "../../components/design/Section";
import {useRouter} from "next/router";
import useSWR from "swr";
import { ArrowSmallLeftIcon, ArrowSmallRightIcon } from '@heroicons/react/24/solid';

const Calendar: NextPage = () => {

    const router = useRouter();
    const monday = startOfWeek(parse(router.query.date as string, "yyyy-MM-dd", new Date()), { weekStartsOn: 1 });

    const {data: appointments} = useSWR(router.asPath, loadAppointments);
    
    const {t} = useTranslation("common");
    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];
    const days = [1, 2, 3, 4, 5, 6, 7];
    
    function goNextWeek()
    {
        router.push("/calendar/" + format(add(monday, {weeks: 1}), "yyyy-MM-dd"));
    }
    
    function goPreviousWeek()
    {
        router.push("/calendar/" + format(add(monday, {weeks: -1}), "yyyy-MM-dd"));
    }

    useKeyPressEvent('ArrowLeft', () => {
        goPreviousWeek();
    })

    useKeyPressEvent('ArrowRight', () => {
        goNextWeek();
    })

    async function loadAppointments() {
        return await postData<Appointment[]>("/appointments", {
            startDate: formatISO(monday),
            endDate: formatISO(add(monday, {days: 8}))
        });
    }

    const getDayClassName = (d: number) => {
        return styles["day" + d];
    }

    function getHourClassName(h: number, m: number) {
        if (m < 30) {
            return styles["hour" + h];
        }

        return styles["hourEnd" + h];
    }

    const getHourEndClassName = (h: number) => {
        return styles["hourEnd" + h];
    }

    const getDurationClassName = (d: number) => {
        d = Math.round(d / 30) * 30;
        return styles["duration" + d];
    }

    const addAppointment = (d: number, h: number, m: number) => {
        const startDate = add(new Date(monday.getFullYear(), monday.getMonth(), monday.getDate()), {days: d - 1, hours: h, minutes: m});
        router.push("/appointments/new?startDate=" + encodeURIComponent(formatISO(startDate)));
    }

    const swipeHandlers = useSwipeable({
        onSwipedLeft: () => goNextWeek(),
        onSwipedRight: () => goPreviousWeek(),
    });

    return <ContentContainer>
        <>
            <Mobile breakpoint={"SM"} visible={true}>
                <div className={styles.mobileWarning}>{t("alerts.mobileWarning")}</div>
            </Mobile>

            <Mobile breakpoint={"SM"}>
                <Section>
                    <div {...swipeHandlers}>
                        <h1>{t("pages.calendar.title")} {format(monday, "EEEE dd MMMM yyyy", {locale: getLocale()})}</h1>


                        <div className={styles.navigationHeader}>
                            <div className={styles.navigationLeft} onClick={goPreviousWeek}><ArrowSmallLeftIcon/></div>
                            <div className={styles.navigationRight} onClick={goNextWeek}><ArrowSmallRightIcon/></div>
                        </div>
                        <div className={styles.calendar}>
                            <div className={styles.hour + " " + styles.hour0}></div>

                            {days.map(d => <div className={styles.headerDay + " " + getDayClassName(d)} key={d}>
                                {t("days.short.day" + d)} {format(add(monday, {days: d - 1}), "dd MMM", {locale: getLocale()})}
                            </div>)}

                            {hours.map(h => <div key={h} className={styles.hour + " " + getHourClassName(h, 0)}>{h}H</div>)}

                            {days.map(d => <React.Fragment key={d}>
                                {hours.map(h => <React.Fragment key={h}>
                                    <div className={styles.dayAction + " " + styles.dayHourFirst + " " + getHourClassName(h, 0) + " " + getDayClassName(d)} onClick={() => addAppointment(d, h, 0)}></div>
                                    <div className={styles.dayAction + " " + styles.dayHourSecond + " " + getHourEndClassName(h) + " " + getDayClassName(d)} onClick={() => addAppointment(d, h, 30)}></div>
                                </React.Fragment>)}
                            </React.Fragment>)}

                            {appointments?.map(m =>
                                <div className={styles.calendarItem + " " + getHourClassName(parseISO(m.startDate).getHours(), parseISO(m.startDate).getMinutes()) + " " + getDayClassName(parseISO(m.startDate).getDay()) + " " + getDurationClassName(m.duration)} key={m.id}
                                     onClick={() => router.push("/appointments/" + m.id)}
                                     style={{backgroundColor: m.backgroundColor ?? ""}}>
                                    <div>{m.title?.slice(0, 30)}</div>
                                </div>)}
                        </div>
                    </div>
                </Section>
            </Mobile>
        </>
    </ContentContainer>
}

export default Calendar;