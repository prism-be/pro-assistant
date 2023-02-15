import useSWR from "swr";
import Section from "../design/Section";
import {postData} from "../../lib/ajaxHelper";
import {Appointment} from "../../lib/contracts";
import useTranslation from "next-translate/useTranslation";
import {add, format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {useRouter} from "next/router";

export interface Props {
    contactId: string;
}

export const ContactAppointments = (props: Props) => {
    async function loadAppointments(): Promise<Appointment[]> {
        const data = await postData<Appointment[]>("/appointments", {contactId: props.contactId});
        return data?.reverse() ?? [];
    }

    const {data: appointments} = useSWR<Appointment[]>('/api/contacts/' + props.contactId + '/appointments', loadAppointments);

    const {t} = useTranslation('common');
    const router = useRouter();

    async function displayAppointment(id: string | null) {
        await router.push("/appointments/" + id);
    }

    return <Section>
        <h2>{t("pages.contacts.details.appointments.title")}</h2>
        <div className={"styles.list"}>
            {appointments?.map(m => <div key={m.id} className={"componentStyles.appointment"} onClick={() => displayAppointment(m.id)}>
                <div className={"componentStyles.typeColor"} style={{backgroundColor: m.backgroundColor ?? ""}}></div>
                <div className={"componentStyles.title"}>
                    {m.title}
                    <span className={"styles.badge"}>
                        {t("options.appointments.state" + m.state)}
                    </span>
                    <span className={"styles.badge"}>
                        {t("options.payments.state" + m.payment)}
                    </span>
                </div>
                <div className={"componentStyles.date"}>
                    {format(parseISO(m.startDate), "EEEE dd MMMM yyyy", {locale: getLocale()})}
                </div>

                <div className={"componentStyles.hour"}>
                    {format(parseISO(m.startDate), "HH:mm", {locale: getLocale()})}
                    &nbsp;-&nbsp;
                    {format(add(parseISO(m.startDate), {minutes: m.duration}), "HH:mm", {locale: getLocale()})}
                </div>
            </div>)}
        </div>
        <>
            {appointments?.length === 0 && <div className={"componentStyles.noAppointments"}>
                {t("pages.contacts.details.appointments.noAppointments")}
            </div>}
        </>

    </Section>
}