import {Appointment, DocumentConfiguration} from "../../lib/contracts"
import Section from "../design/Section";
import {deleteDataWithBody, downloadDocument, generateDocument} from "../../lib/ajaxHelper";
import useSWR, {mutate} from "swr";
import {useCallback, useState} from "react";
import useTranslation from "next-translate/useTranslation";
import {format, parseISO} from "date-fns";
import {getLocale} from "../../lib/localization";
import {ArrowDownTrayIcon, CheckIcon, TrashIcon} from '@heroicons/react/24/outline';

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

        <>{appointment.documents.length === 0 && <div className={""}>
            {t("pages.appointment.documents.no-documents")}
        </div>}</>

        <div className={""}>
            {appointment.documents.map(d => <div key={d.id} className={""}>
                <div className={""}>{d.title}</div>
                <div className={""}>
                    (
                    {t("pages.appointment.documents.generated")}
                    {format(parseISO(d.date), "dd/MM/yy HH:mm", {locale: getLocale()})}
                    )
                </div>
                <div className={""}>
                    <div onClick={() => startDownloadDocument(d.id)} className={""}>
                        <ArrowDownTrayIcon/>
                    </div>

                    <div onClick={() => startDeleteDocument(d.id)} className={""}>
                        <TrashIcon/>
                    </div>
                </div>
            </div>)}
        </div>

        <div className={""}>
            <select onChange={(e) => setDocument(e.target.value)}>
                <option value={""}>{t("pages.appointment.documents.generate")}</option>
                {documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
            <div onClick={() => startGenerateDocument()} className={""}>
                <CheckIcon/>
            </div>
        </div>

    </Section>
}