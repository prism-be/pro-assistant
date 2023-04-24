import Section from "../design/Section";
import useSWR, {mutate} from "swr";
import {useCallback, useState} from "react";
import {useTranslation} from "react-i18next";
import {format, parseISO} from "date-fns";
import {ArrowDownTrayIcon, CheckIcon, TrashIcon} from "@heroicons/react/24/outline";
import {Appointment, DocumentConfiguration, DownloadReference} from "@/libs/models";
import {deleteData, generateDocument, postData} from "@/libs/http";
import {getLocale} from "@/libs/localization";

interface Props {
    appointment: Appointment;
}

export const GeneratedDocuments = ({ appointment }: Props) => {
    const { data: documents } = useSWR<DocumentConfiguration[]>("/data/document-configurations");
    const [document, setDocument] = useState<string>();
    const { t } = useTranslation("common");

    const startGenerateDocument = useCallback(async () => {
        if (!document || document === "" || appointment?.id == null) {
            return;
        }

        await generateDocument(document, appointment.id);
        await mutate("/data/appointments/" + appointment.id);
    }, [document]);

    const startDeleteDocument = useCallback(
        async (appointmentId: string, documentId: string) => {
            if (confirm(t("confirmations.deleteDocument").toString())) {
                await deleteData(`/document/${appointmentId}/${documentId}`);
                await mutate("/data/appointments/" + appointment.id);
            }
        },
        [appointment]
    );

    async function startDownloadDocument(id: string) {
        console.log( id);
        const reference = await postData<DownloadReference>(`/document/download/${id}`, { });
        window.open(`/api/document/download/${id}/${reference.id}`, "_blank")
    }

    return (
        <Section>
            <h2>{t("pages.appointment.documents.title")}</h2>

            <>
                {appointment.documents.length === 0 && (
                    <div className={"p-4 italic text-center"}>{t("pages.appointment.documents.no-documents")}</div>
                )}
            </>

            <div>
                {appointment.documents.map((d) => (
                    <div key={d.id} className={"flex border-b last:border-0 pb-2"}>
                        <div className={"grow py-2"}>
                            <div>{d.title}</div>
                            <div>
                                ({t("pages.appointment.documents.generated")}
                                {format(parseISO(d.date), "dd/MM/yy HH:mm", {
                                    locale: getLocale(),
                                })}
                                )
                            </div>
                        </div>
                        <div className={"col-start-11 row-span-2 row-start-1"}>
                            <div
                                onClick={() => startDownloadDocument(d.id)}
                                className={"w-10 p-2 cursor-pointer"}
                            >
                                <ArrowDownTrayIcon />
                            </div>

                            <div onClick={() => startDeleteDocument(appointment.id, d.id)} className={"w-10 p-2 cursor-pointer"}>
                                <TrashIcon />
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <div className={"flex"}>
                <select
                    className={"w-full block p-2 outline-0 border border-gray-100"}
                    onChange={(e) => setDocument(e.target.value)}
                >
                    <option value={""}>{t("pages.appointment.documents.generate")}</option>
                    {documents?.map((d) => (
                        <option key={d.id} value={d.id ?? ''}>
                            {d.name}
                        </option>
                    ))}
                </select>
                <div
                    onClick={() => startGenerateDocument()}
                    className={"w-12 h-12 bg-primary p-2 text-white cursor-pointer"}
                >
                    <CheckIcon />
                </div>
            </div>
        </Section>
    );
};
