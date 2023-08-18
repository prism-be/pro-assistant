import {NextPage} from "next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import {useTranslation} from "react-i18next";
import {useMemo, useState} from "react";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";
import {useRouter} from "next/router";
import useSWR from "swr";
import {AccountingDocument} from "@/libs/models";
import {HeaderTitleWithAction} from "@/components/design/HeaderTitleWithAction";
import {useForm} from "react-hook-form";
import InputText from "@/components/forms/InputText";
import {Popup} from "@/components/Pops";
import InputDate from "@/components/forms/InputDate";
import {format, parse, parseISO} from "date-fns";
import Button from "@/components/forms/Button";
import {postData} from "@/libs/http";
import {formatAmount} from "@/libs/formats";
import {PencilSquareIcon, TrashIcon} from "@heroicons/react/24/outline";

const Documents: NextPage = () => {
    const {t} = useTranslation("accounting");
    const router = useRouter();

    const {
        data: documents,
        mutate: mutateDocuments
    } = useSWR<AccountingDocument[]>("/data/accounting/documents/" + (router.query.year ?? new Date().getFullYear()));

    const sortedDocuments = useMemo(() => {
        return documents ? documents.sort((a, b) => parseISO(a.date).getTime() - parseISO(b.date).getTime()) : [];
    }, [documents]);

    const [selectedDocument, setSelectedDocument] = useState<AccountingDocument | null>(null);
    const [editing, setEditing] = useState<boolean>(false);

    const {register, setValue, getValues, handleSubmit, formState: {errors}} = useForm();

    async function setYear(delta: number) {
        let year = parseInt(router.query.year as string);
        year += delta;

        await router.push("/accounting/documents/" + year);
    }

    function addDocument() {
        setSelectedDocument(null);
        setValue("date", format(new Date(), "dd/MM/yyyy"));
        setValue("title", "");
        setValue("amount", 0);
        setEditing(true);
    }

    async function onSaveDocument(data: any) {
        data.amount = parseFloat(data.amount);
        data.date = parse(data.date, "dd/MM/yyyy", new Date());
        if (selectedDocument) {
            data.id = selectedDocument.id;
            await postData("/data/accounting/documents/update", data);
        } else {
            data.id = "";
            await postData("/data/accounting/documents/insert", data);
        }

        await mutateDocuments();
        setEditing(false);
        setSelectedDocument(null);
    }

    function getDocumentType(document: AccountingDocument) {
        if (document.amount > 0) {
            return t("documents.types.income");
        }

        return t("documents.types.expense");
    }

    function startEditing(document: AccountingDocument) {
        setSelectedDocument(document);
        setValue("date", format(parseISO(document.date), "dd/MM/yyyy"));
        setValue("title", document.title);
        setValue("amount", formatAmount(document.amount));
        setEditing(true);
    }

    async function deleteDocument(document: AccountingDocument) {
        if (confirm(t("documents.confirmDelete") + document.title)) {
            await postData("/data/accounting/documents/delete", document);
            await mutateDocuments();
        }
    }

    return <ContentContainer>
        <Section>
            <HeaderTitleWithAction
                title={t("documents.title")}
                action={() => addDocument()}
                actionText={t("common:actions.add")}
            />

            <>
                {editing && <Popup>
                    <form>
                        <div className={"grid grid-cols-2 gap-2"}>
                            <div className={"col-span-2"}>
                                <InputDate
                                    label={t("documents.headers.date")}
                                    name={"date"}
                                    type={"text"}
                                    required={true}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.date}
                                />
                            </div>
                            <div className={"col-span-2"}>
                                <InputText
                                    label={t("documents.headers.title")}
                                    name={"title"}
                                    type={"text"}
                                    required={true}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.title}
                                />
                            </div>
                            <div className={"col-span-2"}>
                                <InputText
                                    label={t("documents.headers.amount")}
                                    name={"amount"}
                                    type={"text"}
                                    required={true}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.amount}
                                />
                            </div>
                            <Button
                                text={t("common:actions.cancel")}
                                onClick={() => setEditing(false)}
                                secondary={true}
                            />
                            <Button text={t("common:actions.save")} onClick={handleSubmit(onSaveDocument)}/>
                        </div>
                    </form>
                </Popup>}
            </>

            <div className={"grid grid-cols-8 cursor-pointer"}>
                <div className={"col-start-1 w-8 m-auto text-primary"}
                     onClick={() => setYear(-1)}>
                    <ArrowSmallLeftIcon/>
                </div>

                <h1 className={"text-center col-span-6"}>
                    {router.query.year}
                </h1>

                <div className={"col-start-8 1 w-8 m-auto text-primary"}
                     onClick={() => setYear(1)}>
                    <ArrowSmallRightIcon/>
                </div>
            </div>

            <div className={"grid grid-cols-8 gap-2 mt-4"}>
                <div className={"font-bold col-span-6"}>{t("documents.headers.title")}</div>
                <div className={"font-bold col-span-1"}>{t("documents.headers.type")}</div>
                <div className={"font-bold col-span-1 text-right"}>{t("documents.headers.amount")}</div>
            </div>

            <>
                {documents?.length === 0 && <div className={"text-center p-3 italic"}>
                    {t("documents.noDocuments")}
                </div>}
            </>
            
            <>
                {sortedDocuments?.map((document) => <div key={document.id}
                                                         className={"grid grid-cols-8 gap-2" + (document.amount < 0 ? " text-red-700" : " text-green-700")}>
                    <div className={"col-span-6 flex"}>
                        <a className={"w-6 cursor-pointer"} onClick={() => startEditing(document)}>
                            {" "}
                            <PencilSquareIcon/>{" "}
                        </a>
                        <a className={"w-6 ml-2 cursor-pointer"} onClick={() => deleteDocument(document)}>
                            {" "}
                            <TrashIcon/>{" "}
                        </a>
                        <div className={"pl-2"}>{format(new Date(document.date), "dd/MM/yyyy")} - {document.title}</div>
                    </div>
                    <div className={"col-span-1"}>{getDocumentType(document)}</div>
                    <div className={"col-span-1 text-right"}>{formatAmount(document.amount)} &euro;</div>
                </div>)}
            </>

        </Section>
    </ContentContainer>
}

export default Documents;