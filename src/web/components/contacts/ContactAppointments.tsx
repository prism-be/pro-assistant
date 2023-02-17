import useSWR from "swr";
import Section from "../design/Section";
import {postData} from "../../lib/ajaxHelper";
import {Appointment} from "../../lib/contracts";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";
import {AppointmentsList} from "../appointments/AppointmentsList";

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
            <AppointmentsList appointments={appointments ?? []} onClick={(m) => displayAppointment(m.id)}/>
        </div>
        <>
            {appointments?.length === 0 && <div className={"componentStyles.noAppointments"}>
                {t("pages.contacts.details.appointments.noAppointments")}
            </div>}
        </>

    </Section>
}