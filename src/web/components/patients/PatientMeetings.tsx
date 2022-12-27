import useSWR from "swr";
import ContentContainer from "../design/ContentContainer";
import Section from "../design/Section";
import {postData} from "../../lib/ajaxHelper";
import {IMeeting} from "../../lib/contracts";
import useTranslation from "next-translate/useTranslation";

export interface Props {
    patientId: string;
}

export const PatientMeetings = (props: Props) => {
    async function loadMeetings() {
        return await postData<IMeeting[]>("/meetings", {patientId: props.patientId});
    }

    const {data: meetings, mutate: mutateMeetings} = useSWR('/api/patients/' + props.patientId + '/meetings', loadMeetings);

    const {t} = useTranslation('common');

    return <Section>
        <h2>{t("pages.patients.details.meetings.title")}</h2>
        <div>
            {meetings?.map(m => <div key={m.id}>{m.title}</div>)}
        </div>
    </Section>
}