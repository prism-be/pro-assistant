import styles from "../../styles/pages/appointment.module.scss";

import {DocumentConfiguration, Appointment} from "../../lib/contracts"
import Section from "../design/Section";
import {deleteDataWithBody, downloadDocument, generateDocument} from "../../lib/ajaxHelper";
import useSWR, {mutate} from "swr";
import {useCallback, useState} from "react";
import useTranslation from "next-translate/useTranslation";
import {format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {TrashIcon, CheckIcon, ArrowDownTrayIcon} from '@heroicons/react/24/outline';

interface Props {
    appointment: Appointment;
}

export const GeneratedDocuments = ({appointment}: Props) => {

    const {data: documents} = useSWR<DocumentConfiguration[]>("/documents-configuration");
    const [document, setDocument] = useState<string>();
    const {t} = useTranslation('common');

    const startGenerateDocument = useCallback(async () => {
        if (!document || document === "" || appointment?.id == null) {
            return;
        }

        await generateDocument(document, appointment.id);
        await mutate("/appointment/" + appointment.id);
    }, [document]);


    const startDownloadDocument = useCallback(async (documentId: string) => {
        await downloadDocument(documentId);
    }, []);

    const startDeleteDocument = useCallback(async (documentId: string) => {
        if (confirm(t("confirmations.deleteDocument"))) {
            await deleteDataWithBody("/document", {id: documentId, appointmentId: appointment.id});
            await mutate("/appointment/" + appointment.id);
        }
    }, [appointment]);

    return <Section>
        <h2>{t("pages.appointment.documents.title")}</h2>

        <>{appointment.documents.length === 0 && <div className={styles.noDocuments}>
            {t("pages.appointment.documents.no-documents")}
        </div>}</>

        <>
            {appointment.documents.map(d => <div key={d.id} className={styles.document}>
                <div className={styles.documentTitle}>{d.title}</div>
                <div className={styles.documentDate}>
                    (
                    {t("pages.appointment.documents.generated")}
                    {format(parseISO(d.date), "dd/MM/yy HH:mm", {locale: getLocale()})}
                    )
                </div>
                <div className={styles.documentAction}>
                    <div onClick={() => startDownloadDocument(d.id)} className={styles.documentSave}>
                        <ArrowDownTrayIcon/>
                    </div>

                    <div onClick={() => startDeleteDocument(d.id)} className={styles.documentSave}>
                        <TrashIcon/>
                    </div>
                </div>
            </div>)}
        </>

        <div className={styles.documents}>
            <select onChange={(e) => setDocument(e.target.value)}>
                <option value={""}>{t("pages.appointment.documents.generate")}</option>
                {documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
            <div onClick={() => startGenerateDocument()} className={styles.documentsSave}>
                <CheckIcon/>
            </div>
        </div>

    </Section>
}