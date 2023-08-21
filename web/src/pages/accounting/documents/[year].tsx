﻿import {NextPage} from "next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import {useTranslation} from "react-i18next";
import { useMemo, useState} from "react";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";
import {useRouter} from "next/router";
import useSWR from "swr";
import {AccountingDocument, NextNumber} from "@/libs/models";
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
import InputSelect from "@/components/forms/InputSelect";

const Documents: NextPage = () => {
    const {t} = useTranslation("accounting");
    const router = useRouter();

    const {
        data: documents,
        mutate: mutateDocuments
    } = useSWR<AccountingDocument[]>("/data/accounting/documents/" + (router.query.year ?? new Date().getFullYear()));

    const {
        data: nextNumber,
        mutate: mutateNextNumber
    } = useSWR<NextNumber>("/data/accounting/documents/next-number/" + (router.query.year ?? new Date().getFullYear()));

    const sortedDocuments = useMemo(() => {
        return documents ? documents.sort((a, b) => parseISO(a.date).getTime() - parseISO(b.date).getTime()) : [];
    }, [documents]);

    const [selectedDocument, setSelectedDocument] = useState<AccountingDocument | null>(null);
    const [editing, setEditing] = useState<boolean>(false);
    const [displayDocumentNumber, setDisplayDocumentNumber] = useState<boolean>(false);

    const {register, setValue, handleSubmit, formState: {errors}} = useForm();

    async function setYear(delta: number) {
        let year = parseInt(router.query.year as string);
        year += delta;

        await router.push("/accounting/documents/" + year);
    }

    function addDocument() {
        setSelectedDocument(null);
        setValue("date", format(new Date(), "dd/MM/yyyy"));
        setValue("title", "");
        setValue("reference", "");
        setValue("type", "income");
        setValue("amount", 0);
        setValue("documentNumberChoice", "generate")
        setValue("documentNumber", nextNumber?.number);
        setDisplayDocumentNumber(true);
        setEditing(true);
    }

    async function onSaveDocument(data: any) {
        data.amount = parseFloat(data.amount);
        
        if (data.type === "expense") {
            data.amount *= -1;
        }
        
        data.date = parse(data.date, "dd/MM/yyyy", new Date());
        if (selectedDocument) {
            data.id = selectedDocument.id;
            await postData("/data/accounting/documents/update", data);
        } else {
            data.id = "";
            await postData("/data/accounting/documents/insert", data);
        }

        await mutateNextNumber();
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
        setValue("reference", document.reference);

        setValue("amount", formatAmount(Math.abs(document.amount)));
        if (document.amount > 0) {
            setValue("type", "income");
        } else {
            setValue("type", "expense");
        }

        if(document.documentNumber) {
            setValue("documentNumberChoice", "generate");
            setValue("documentNumber", document.documentNumber);
            setDisplayDocumentNumber(true);
        } else {
            setValue("documentNumberChoice", "noGenerate");
            setValue("documentNumber", "");
            setDisplayDocumentNumber(false);
        }

        setEditing(true);
    }

    async function deleteDocument(document: AccountingDocument) {
        if (confirm(t("documents.confirmDelete") + document.title)) {
            await postData("/data/accounting/documents/delete", document);
            await mutateDocuments();
        }
    }

    function switchDisplayDocumentNumber(action:string) {
        if (action === "generate") {
            setValue("documentNumber", nextNumber?.number);
            setDisplayDocumentNumber(true);
        } else {
            setValue("documentNumber", "");
            setDisplayDocumentNumber(false);
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
                                    label={t("documents.headers.reference")}
                                    name={"reference"}
                                    type={"text"}
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.reference}
                                />
                            </div>
                            <div className={"col-span-2"}>
                                <InputSelect
                                    label={t("documents.headers.type")}
                                    name={"type"}
                                    required={true}
                                    register={register}
                                    error={errors.type}
                                    options={[
                                        {value: "income", text: t("documents.types.income")},
                                        {value: "expense", text: t("documents.types.expense")}
                                    ]}
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
                            <div className={"col-span-2"}>
                                <InputSelect
                                    label={t("documents.documentNumber.title")}
                                    name={"documentNumberChoice"}
                                    required={true}
                                    register={register}
                                    error={errors.documentNumberChoice}
                                    options={[
                                        {value: "generate", text: t("documents.documentNumber.generate")},
                                        {value: "noGenerate", text: t("documents.documentNumber.noGenerate")}
                                    ]}
                                    onChange={(e) => { switchDisplayDocumentNumber(e); }}
                                />
                            </div>
                            {displayDocumentNumber && <div className={"col-span-2"}>
                                <InputText
                                    label={t("documents.documentNumber.title")}
                                    name={"documentNumber"}
                                    type={"text"}
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.documentNumber}
                                />
                            </div>}
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
                        <div className={"pl-2"}>
                            {format(new Date(document.date), "dd/MM/yyyy")} - {document.title}
                            <>{document.reference?.length > 0 && <span className={"italic pl-2"}>({document.reference})</span>}</>
                        </div>
                    </div>
                    <div className={"col-span-1"}>{getDocumentType(document)}</div>
                    <div className={"col-span-1 text-right"}>{formatAmount(document.amount)} &euro;</div>
                </div>)}
            </>

        </Section>
    </ContentContainer>
}

export default Documents;