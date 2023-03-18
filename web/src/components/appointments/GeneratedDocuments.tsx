import Section from "../design/Section";
import useSWR, { mutate } from "swr";
import { useCallback, useState } from "react";
import useTranslation from "next-translate/useTranslation";
import { format, parseISO } from "date-fns";
import { ArrowDownTrayIcon, CheckIcon, TrashIcon } from "@heroicons/react/24/outline";
import { Appointment, DocumentConfiguration } from "@/libs/models";
import { deleteDataWithBody, generateDocument } from "@/libs/http";
import { getLocale } from "@/libs/localization";
import { DocumentRequest } from "@/modules/documents/types";

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
        async (documentId: string) => {
            if (confirm(t("confirmations.deleteDocument"))) {
                await deleteDataWithBody("/documents/delete", {
                    documentId,
                    appointmentId: appointment.id,
                } as DocumentRequest);
                await mutate("/data/appointments/" + appointment.id);
            }
        },
        [appointment]
    );

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
                                onClick={() => window.open(`/api/documents/${d.id}`)}
                                className={"w-10 p-2 cursor-pointer"}
                            >
                                <ArrowDownTrayIcon />
                            </div>

                            <div onClick={() => startDeleteDocument(d.id)} className={"w-10 p-2 cursor-pointer"}>
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
                        <option key={d.id} value={d.id}>
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
