import {DocumentConfiguration, Meeting} from "../../lib/contracts"
import Section from "../design/Section";
import {generateDocument} from "../../lib/ajaxHelper";
import styles from "../../styles/pages/meeting.module.scss";
import {Save} from "../icons/Save";
import useSWR, {mutate} from "swr";
import {useState} from "react";
import useTranslation from "next-translate/useTranslation";

interface Props {
    meeting: Meeting;
}

export const GeneratedDocuments = ({meeting}: Props) => {

    const {data: documents} = useSWR<DocumentConfiguration[]>("/documents-configuration");
    const [document, setDocument] = useState<string>();
    const {t} = useTranslation('common');
    
    async function startDownloadDocument() {
        if (!document || document === "" || meeting?.id == null) {
            return;
        }

        await generateDocument(document, meeting.id);
        await mutate("/meeting/" + meeting.id);
    }
    
    return <Section>
        <h2>{t("pages.meeting.documents.title")}</h2>

        <div className={styles.documents}>
            <select onChange={(e) => setDocument(e.target.value)}>
                <option value={""}>{t("pages.meeting.documents.generate")}</option>
                {documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
            <div onClick={() => startDownloadDocument()} className={styles.documentsSave}>
                <Save/>
            </div>
        </div>
        
    </Section>
}