import Section from "../../components/design/Section";
import ContentContainer from "../../components/design/ContentContainer";
import {useTranslation} from "react-i18next";
import {add, format, formatISO, parse, parseISO} from "date-fns";
import React from "react";
import {useSwipeable} from "react-swipeable";
import useSWR from "swr";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";
import {postData} from "@/libs/http";
import {Appointment} from "@/libs/models";
import {getLocale} from "@/libs/localization";
import {AppointmentStateIcon} from "@/components/appointments/AppointmentStateIcon";
import {defaultColor} from "@/libs/constants";
import {useLoaderData, useNavigate} from "react-router-dom";

interface Query {
    date: string;
}

const Agenda = () => {
    const navigate = useNavigate();

    const {date} = useLoaderData() as Query;
    const day: Date = parse(date, "yyyy-MM-dd", new Date());
    const {t} = useTranslation("common");

    const {data: appointments} = useSWR("/appointments/" + date, loadAppointments);

    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];

    async function loadAppointments() {
        return await postData<Appointment[]>("/data/appointments/search", [
            {
                field: "StartDate",
                operator: "gte",
                value: formatISO(day)
            }
            ,
            {
                field: "StartDate",
                operator: "lt",
                value: formatISO(add(day, {days: 1}))
            }
        ]);
    }

    function changeDay(delta: number) {
        const newDay = add(day, {days: delta});
        navigate("/agenda/" + format(newDay, "yyyy-MM-dd"));
    }

    const swipeHandlers = useSwipeable({
        onSwipedLeft: () => changeDay(1),
        onSwipedRight: () => changeDay(-1),
    });

    function getHourRowStart(h: number, m: number) {
        if (m < 30) {
            return h * 2;
        }

        return getHourRowEnd(h);
    }

    function getHourRowEnd(h: number) {
        return h * 2 + 1;
    }

    function getDurationClassName(d: number) {
        d = Math.round(d / 30) * 30;
        return "span " + d / 30;
    }

    function addAppointment(h: number, m: number) {
        const startDate = add(new Date(day.getFullYear(), day.getMonth(), day.getDate()), {hours: h, minutes: m});
        navigate("/appointments/new?startDate=" + encodeURIComponent(formatISO(startDate)));
    }

    function previousDay() {
        changeDay(-1);
    }

    function nextDay() {
        changeDay(1);
    }

    return (
        <ContentContainer>
            <Section>
                <div {...swipeHandlers} className={"grid grid-cols-8 cursor-pointer"}>
                    <div className={"col-start-1 w-8 m-auto text-primary"} onClick={previousDay}>
                        <ArrowSmallLeftIcon/>
                    </div>

                    <h1 className={"hidden text-center md:block col-span-6"}>
                        {t("pages.agenda.title")} {format(day, "EEEE d MMMM yyyy", {locale: getLocale()})}
                    </h1>
                    <h1 className={"text-center md:hidden col-span-6"}>
                        {format(day, "EEEE", {locale: getLocale()})}
                        <br/>
                        {format(day, "d/MM/yy", {locale: getLocale()})}
                    </h1>

                    <div className={"col-start-8 1 w-8 m-auto text-primary"} onClick={nextDay}>
                        <ArrowSmallRightIcon/>
                    </div>

                    {appointments?.length === 0 && (
                        <div className={"col-span-8 p-2 text-center italic"}>{t("pages.agenda.noAppointment")}</div>
                    )}

                    <div className={"h-4"}/>

                    {hours.map((h) => (
                        <React.Fragment key={h}>
                            <div
                                className={
                                    "col-start-1 row-span-2 text-right border-r border-b pr-2 text-sm leading-6"
                                }
                                style={{gridRowStart: getHourRowStart(h, 0)}}
                            >
                                {h}H
                            </div>
                            <div
                                className={"col-start-2 col-span-7 h-6 border-b border-dashed"}
                                style={{gridRowStart: getHourRowStart(h, 0)}}
                                onClick={() => addAppointment(h, 0)}
                            ></div>
                            <div
                                className={"col-start-2 col-span-7 h-6 border-b"}
                                style={{gridRowStart: getHourRowEnd(h)}}
                                onClick={() => addAppointment(h, 30)}
                            ></div>
                        </React.Fragment>
                    ))}

                    {appointments?.map((a) => (
                        <div
                            className={"col-start-2 col-span-7 text-sm pl-1 leading-6 text-white"}
                            key={a.id}
                            onClick={() => navigate("/appointments/" + a.id)}
                            style={{
                                backgroundColor: a.backgroundColor ?? defaultColor,
                                gridRowStart: getHourRowStart(
                                    parseISO(a.startDate).getHours(),
                                    parseISO(a.startDate).getMinutes()
                                ),
                                gridRowEnd: getDurationClassName(a.duration),
                            }}
                        >
                            <div className={"relative"}>
                                {a.title?.slice(0, 40)}
                                <AppointmentStateIcon
                                    payment={a.payment}
                                    state={a.state}
                                    backgroundColor={a.backgroundColor ?? ""}
                                />
                            </div>
                        </div>
                    ))}
                </div>
            </Section>
        </ContentContainer>
    );
};

export default Agenda;
