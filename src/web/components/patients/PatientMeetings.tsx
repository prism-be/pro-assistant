import styles from '../../styles/components/patients/patient-meetings.module.scss';
import useSWR from "swr";
import Section from "../design/Section";
import {postData} from "../../lib/ajaxHelper";
import {IMeeting} from "../../lib/contracts";
import useTranslation from "next-translate/useTranslation";
import {add, format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {useRouter} from "next/router";

export interface Props {
    patientId: string;
}

export const PatientMeetings = (props: Props) => {
    async function loadMeetings() {
        const data = await postData<IMeeting[]>("/meetings", {patientId: props.patientId});
        return data?.reverse();
    }

    const {data: meetings} = useSWR('/api/patients/' + props.patientId + '/meetings', loadMeetings);

    const {t} = useTranslation('common');
    const router = useRouter();

    async function displayMeeting(id: string | undefined) {
        await router.push("/meetings/" + id);
    }

    return <Section>
        <h2>{t("pages.patients.details.meetings.title")}</h2>
        <div>
            {meetings?.map(m => <div key={m.id} className={styles.meeting} onClick={() => displayMeeting(m.id)}>
                <div className={styles.typeColor} style={{backgroundColor: m.backgroundColor}}></div>
                <div className={styles.title}>
                    {m.title}
                    <span className={styles.badge}>
                        {t("options.meetings.state" + m.state)}
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
            {meetings?.length === 0 && <div className={styles.noMeetings}>
                {t("pages.patients.details.meetings.noMeetings")}
            </div>}
        </>

    </Section>
}