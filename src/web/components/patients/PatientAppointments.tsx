import styles from '../../styles/components/patients/patient-appointments.module.scss';
import useSWR from "swr";
import Section from "../design/Section";
import {postData} from "../../lib/ajaxHelper";
import {Appointment} from "../../lib/contracts";
import useTranslation from "next-translate/useTranslation";
import {add, format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {useRouter} from "next/router";

export interface Props {
    patientId: string;
}

export const PatientAppointments = (props: Props) => {
    async function loadAppointments(): Promise<Appointment[]> {
        const data = await postData<Appointment[]>("/appointments", {patientId: props.patientId});
        return data?.reverse() ?? [];
    }

    const {data: appointments} = useSWR<Appointment[]>('/api/patients/' + props.patientId + '/appointments', loadAppointments);

    const {t} = useTranslation('common');
    const router = useRouter();

    async function displayAppointment(id: string | null) {
        await router.push("/appointments/" + id);
    }

    return <Section>
        <h2>{t("pages.patients.details.appointments.title")}</h2>
        <div>
            {appointments?.map(m => <div key={m.id} className={styles.appointment} onClick={() => displayAppointment(m.id)}>
                <div className={styles.typeColor} style={{backgroundColor: m.backgroundColor ?? ""}}></div>
                <div className={styles.title}>
                    {m.title}
                    <span className={styles.badge}>
                        {t("options.appointments.state" + m.state)}
                    </span>
                    <span className={styles.badge}>
                        {t("options.payments.state" + m.payment)}
                    </span>
                </div>
                <div className={styles.date}>
                    {format(parseISO(m.startDate), "EEEE dd MMMM yyyy", {locale: getLocale()})}
                </div>

                <div className={styles.hour}>
                    {format(parseISO(m.startDate), "HH:mm", {locale: getLocale()})}
                    &nbsp;-&nbsp;
                    {format(add(parseISO(m.startDate), {minutes: m.duration}), "HH:mm", {locale: getLocale()})}
                </div>
            </div>)}
        </div>
        <>
            {appointments?.length === 0 && <div className={styles.noAppointments}>
                {t("pages.patients.details.appointments.noAppointments")}
            </div>}
        </>

    </Section>
}