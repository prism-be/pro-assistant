import styles from "../../styles/pages/meeting.module.scss";

import {DocumentConfiguration, Meeting} from "../../lib/contracts"
import Section from "../design/Section";
import {deleteDataWithBody, downloadDocument, generateDocument} from "../../lib/ajaxHelper";
import {Save} from "../icons/Save";
import useSWR, {mutate} from "swr";
import {useCallback, useState} from "react";
import useTranslation from "next-translate/useTranslation";
import {format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {Delete} from "../icons/Icons";

interface Props {
    meeting: Meeting;
}

export const GeneratedDocuments = ({meeting}: Props) => {

    const {data: documents} = useSWR<DocumentConfiguration[]>("/documents-configuration");
    const [document, setDocument] = useState<string>();
    const {t} = useTranslation('common');

    const startGenerateDocument = useCallback(async () => {
        if (!document || document === "" || meeting?.id == null) {
            return;
        }

        await generateDocument(document, meeting.id);
        await mutate("/meeting/" + meeting.id);
    }, [document]);


    const startDownloadDocument = useCallback(async (documentId: string) => {
        await downloadDocument(documentId);
    }, []);

    const startDeleteDocument = useCallback(async (documentId: string) => {
        if (confirm(t("confirmations.deleteDocument"))) {
            await deleteDataWithBody("/document", {id: documentId, meetingId: meeting.id});
            await mutate("/meeting/" + meeting.id);
        }
    }, [meeting]);

    return <Section>
        <h2>{t("pages.meeting.documents.title")}</h2>

        <>{meeting.documents.length === 0 && <div className={styles.noDocuments}>
            {t("pages.meeting.documents.no-documents")}
        </div>}</>

        <>
            {meeting.documents.map(d => <div key={d.id} className={styles.document}>
                <div className={styles.documentTitle}>{d.title}</div>
                <div className={styles.documentDate}>
                    (
                    {t("pages.meeting.documents.generated")}
                    {format(parseISO(d.date), "dd/MM/yy HH:mm", {locale: getLocale()})}
                    )
                </div>
                <div className={styles.documentAction}>
                    <div onClick={() => startDownloadDocument(d.id)} className={styles.documentSave}>
                        <Save/>
                    </div>

                    <div onClick={() => startDeleteDocument(d.id)} className={styles.documentSave}>
                        <Delete/>
                    </div>
                </div>
            </div>)}
        </>

        <div className={styles.documents}>
            <select onChange={(e) => setDocument(e.target.value)}>
                <option value={""}>{t("pages.meeting.documents.generate")}</option>
                {documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
            <div onClick={() => startGenerateDocument()} className={styles.documentsSave}>
                <Save/>
            </div>
        </div>

    </Section>
}