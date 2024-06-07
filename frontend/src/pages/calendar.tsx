import ContentContainer from "@/components/design/ContentContainer";
import {add, format, formatISO, parse, parseISO, startOfWeek} from "date-fns";
import {useTranslation} from "react-i18next";
import React from "react";
import {useKeyPressEvent} from "react-use";
import {useSwipeable} from "react-swipeable";
import Section from "@/components/design/Section";
import useSWR from "swr";
import {ArrowLeftIcon, ArrowRightIcon} from "@heroicons/react/24/solid";
import {Appointment} from "@/libs/models";
import {postData} from "@/libs/http";
import {getLocale} from "@/libs/localization";
import {AppointmentStateIcon} from "@/components/appointments/AppointmentStateIcon";
import {defaultColor} from "@/libs/constants";
import {useLoaderData, useNavigate} from "react-router-dom";

interface Query {
    date: string;
}

const Calendar = () => {
    const  {date} = useLoaderData() as Query;
    const navigate = useNavigate();
    
    const monday = startOfWeek(parse(date, "yyyy-MM-dd", new Date()), { weekStartsOn: 1 });

    const { data: appointments } = useSWR( `/appointments/${date}`, loadAppointments);

    const { t } = useTranslation("common");
    const hours = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21];
    const days = [1, 2, 3, 4, 5, 6, 7];

    function goNextWeek() {
        navigate("/calendar/" + format(add(monday, { weeks: 1 }), "yyyy-MM-dd"));
    }

    function goPreviousWeek() {
        navigate("/calendar/" + format(add(monday, { weeks: -1 }), "yyyy-MM-dd"));
    }

    useKeyPressEvent("ArrowLeft", () => {
        goPreviousWeek();
    });

    useKeyPressEvent("ArrowRight", () => {
        goNextWeek();
    });

    async function loadAppointments() {
        return await postData<Appointment[]>("/data/appointments/search", [
            {
                field: "StartDate",
                operator: "gte",
                value: monday
            }
            ,
            {
                field: "StartDate",
                operator: "lt",
                value: add(monday, { weeks: 1 })
            }
        ]);
    }

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

    const addAppointment = (d: number, h: number, m: number) => {
        const startDate = add(new Date(monday.getFullYear(), monday.getMonth(), monday.getDate()), {
            days: d - 1,
            hours: h,
            minutes: m,
        });
        navigate("/appointments/new#" + encodeURIComponent(formatISO(startDate)));
    };

    const swipeHandlers = useSwipeable({
        onSwipedLeft: () => goNextWeek(),
        onSwipedRight: () => goPreviousWeek(),
    });

    return (
        <ContentContainer>
            <>
                <div className={"text-orange-600 text-center p-8 m sm:hidden"}>{t("alerts.mobileWarning")}</div>

                <div className={"hidden sm:block"}>
                    <Section>
                        <div className={"grid grid-cols-8 cursor-pointer"} {...swipeHandlers}>
                            <div className={"col-start-1 w-8 m-auto text-primary"} onClick={goPreviousWeek}>
                                <ArrowLeftIcon />
                            </div>
                            <h1 className={"col-span-6 text-center"}>
                                {t("pages.calendar.title")}{" "}
                                {format(monday, "EEEE dd MMMM yyyy", { locale: getLocale() })}
                            </h1>
                            <div className={"col-start-8 1 w-8 m-auto text-primary"} onClick={goNextWeek}>
                                <ArrowRightIcon />
                            </div>

                            <div className={"h-8 col-span-8"}></div>

                            <div
                                className={
                                    "text-sm col-start-1 text-center leading-8 border-b border-b-gray-300 border-r border-r-light"
                                }
                            ></div>
                            {days.map((d) => (
                                <div
                                    className={
                                        "text-xs text-center leading-8 border-b border-b-gray-300 border-r border-r-light"
                                    }
                                    style={{ gridColumnStart: d + 1 }}
                                    key={d}
                                >
                                    {t("days.short.day" + d)}{" "}
                                    {format(add(monday, { days: d - 1 }), "dd MMM", {
                                        locale: getLocale(),
                                    })}
                                </div>
                            ))}

                            {hours.map((h) => (
                                <div
                                    key={h}
                                    className={
                                        "text-sm text-right leading-6 pr-1 border-b border-b-gray-300 border-r border-r-light row-span-2"
                                    }
                                    style={{ gridRowStart: h * 2 }}
                                >
                                    {h}H
                                </div>
                            ))}

                            {days.map((d) => (
                                <React.Fragment key={d}>
                                    {hours.map((h) => (
                                        <React.Fragment key={h}>
                                            <div
                                                className={
                                                    "h-6 hover:bg-light border-r border-r-light border-b border-b-dashed" +
                                                    (d > 5 ? " bg-gray-100" : "")
                                                }
                                                style={{
                                                    gridColumnStart: d + 1,
                                                    gridRowStart: h * 2,
                                                }}
                                                onClick={() => addAppointment(d, h, 0)}
                                            ></div>
                                            <div
                                                className={
                                                    "h-6 hover:bg-light border-r border-r-light border-b border-b-gray-300" +
                                                    (d > 5 ? " bg-gray-100" : "")
                                                }
                                                style={{
                                                    gridColumnStart: d + 1,
                                                    gridRowStart: h * 2 + 1,
                                                }}
                                                onClick={() => addAppointment(d, h, 30)}
                                            ></div>
                                        </React.Fragment>
                                    ))}
                                </React.Fragment>
                            ))}

                            {appointments?.map((a) => (
                                <div
                                    className={"text-xs text-white leading-6 pl-1 lg:text-sm"}
                                    style={{
                                        gridColumnStart: parseISO(a.startDate).getDay() + 1,
                                        gridRowStart:getHourRowStart(
                                                parseISO(a.startDate).getHours(),
                                                parseISO(a.startDate).getMinutes()
                                        ),
                                        backgroundColor: a.backgroundColor ?? defaultColor,
                                        gridRowEnd: getDurationClassName(a.duration),
                                    }}
                                    key={a.id}
                                    onClick={() => navigate("/appointments/" + a.id)}
                                >
                                    <div className={"hidden md:block relative"}>
                                        {a.title?.slice(0, 30)}
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
                </div>
            </>
        </ContentContainer>
    );
};

export default Calendar;
