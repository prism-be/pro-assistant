import { add, format, parseISO } from "date-fns";
import useTranslation from "next-translate/useTranslation";
import { getLocale } from "@/libs/localization";
import { Appointment } from "@/libs/models";

interface Props {
    appointments: Appointment[];
    onClick: (appointment: Appointment) => void;
}

export const AppointmentsList = ({ appointments, onClick }: Props) => {
    const { t } = useTranslation("common");

    return (
        <>
            {appointments?.map((m) => (
                <div
                    key={m._id}
                    className={"grid contact-appointments border-b last:border-0"}
                    onClick={() => onClick(m)}
                >
                    <div className={"row-span-2"} style={{ backgroundColor: m.backgroundColor ?? "" }}></div>
                    <div className={"col-span-3 pt-2 pl-2 lg:col-span-1"}>
                        {m.title}
                        <span className={"inline-block px-2 mx-2 border border-primary rounded text-sm text-primary"}>
                            {t("options.appointments.state" + m.state)}
                        </span>
                        <span className={"inline-block px-2 mx-2 border border-primary rounded text-sm text-primary"}>
                            {t("options.payments.state" + m.payment)}
                        </span>
                    </div>
                    <div className={"col-span-2 pb-2 pl-2 lg:col-span-1 lg:pt-2"}>
                        {format(parseISO(m.startDate), "EEEE dd MMMM yyyy", {
                            locale: getLocale(),
                        })}
                    </div>

                    <div className={"pb-2 lg:pt-2"}>
                        {format(parseISO(m.startDate), "HH:mm", { locale: getLocale() })}
                        &nbsp;-&nbsp;
                        {format(add(parseISO(m.startDate), { minutes: m.duration }), "HH:mm", { locale: getLocale() })}
                    </div>
                </div>
            ))}
        </>
    );
};
