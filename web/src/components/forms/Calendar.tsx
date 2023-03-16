import { add, format, formatISO, startOfMonth } from "date-fns";
import { useEffect, useState } from "react";
import useTranslation from "next-translate/useTranslation";
import { ArrowSmallLeftIcon, ArrowSmallRightIcon } from "@heroicons/react/24/outline";
import { getLocale } from "@/libs/localization";

interface Props {
    value: Date;
    onChange: (date: Date) => void;
    className?: string;
}

export const Calendar = ({ value, onChange, className }: Props) => {
    const { t } = useTranslation("common");
    const [date, setDate] = useState<Date>(value);
    const [month, setMonth] = useState<Date>(startOfMonth(value));
    const [dates, setDates] = useState<Date[]>([]);

    useEffect(() => {
        let c = month;
        let d: Date[] = [];

        while (c.getMonth() === month.getMonth()) {
            d.push(new Date(c));
            c = add(c, { days: 1 });
        }

        setDates(d);
    }, [month]);

    useEffect(() => {
        setDate(value);
        setMonth(startOfMonth(value));
    }, [value]);

    const getClass = (d: Date) => {
        if (
            d.getFullYear() === date.getFullYear() &&
            d.getMonth() === date.getMonth() &&
            d.getDate() === date.getDate()
        ) {
            return "bg-primary text-white";
        }

        return "";
    };

    return (
        <div className={"grid gap-2 grid-cols-7 select-none" + " " + className} title={formatISO(value)}>
            <div
                className={"col-start-1 w-6 text-primary cursor-pointer m-auto"}
                onClick={() => setMonth(add(month, { months: -1 }))}
            >
                <ArrowSmallLeftIcon />
            </div>
            <div className={"font-bold col-span-5 text-center"}>
                {format(month, "MMMM yyyy", { locale: getLocale() })}
            </div>
            <div
                className={"col-start-7 w-6 text-right text-primary cursor-pointer m-auto"}
                onClick={() => setMonth(add(month, { months: 1 }))}
            >
                <ArrowSmallRightIcon />
            </div>
            <div className={"text-center font-bold"}>{t("days.short.day1")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day2")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day3")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day4")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day5")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day6")}</div>
            <div className={"text-center font-bold"}>{t("days.short.day7")}</div>
            {dates.map((d) => (
                <div
                    key={formatISO(d)}
                    className={"text-center cursor-pointer " + getClass(d)}
                    style={{ gridColumnStart: d.getDay() + 1 }}
                    onClick={() => {
                        onChange(d);
                        setDate(d);
                    }}
                >
                    {d.getDate()}
                </div>
            ))}
        </div>
    );
};
