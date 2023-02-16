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
        <div>
            {appointments?.map(m => <div key={m.id} className={"grid contact-appointments border-b last:border-0"} onClick={() => displayAppointment(m.id)}>
                <div className={"row-span-2"} style={{backgroundColor: m.backgroundColor ?? ""}}></div>
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
                    {format(parseISO(m.startDate), "EEEE dd MMMM yyyy", {locale: getLocale()})}
                </div>

                <div className={"pb-2 lg:pt-2"}>
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