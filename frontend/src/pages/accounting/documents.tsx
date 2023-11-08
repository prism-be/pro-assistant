import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import {useTranslation} from "react-i18next";
import { useMemo, useState} from "react";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";
import useSWR from "swr";
import {AccountingDocument, NextNumber} from "@/libs/models";
import {HeaderTitleWithAction} from "@/components/design/HeaderTitleWithAction";
import {useForm} from "react-hook-form";
import InputText from "@/components/forms/InputText";
import {Popup} from "@/components/Pops";
import InputDate from "@/components/forms/InputDate";
import {format, formatISO, parse, parseISO } from "date-fns";
import Button from "@/components/forms/Button";
import {postData} from "@/libs/http";
import {formatAmount} from "@/libs/formats";
import {PencilSquareIcon, TrashIcon} from "@heroicons/react/24/outline";
import InputSelect from "@/components/forms/InputSelect";
import InputTextAutoComplete from "@/components/forms/InputTextAutoComplete";
import {onlyUnique} from "@/libs/text";
import {useLoaderData, useNavigate} from "react-router-dom";

interface Query {
    year: string;
}

const AccountingDocuments = () => {
    const {year} = useLoaderData() as Query;
    const {t} = useTranslation("accounting");
    const navigate = useNavigate();

    const {
        data: documents,
        mutate: mutateDocuments
    } = useSWR<AccountingDocument[]>("/data/accounting/documents/" + (year ?? new Date().getFullYear()));

    const {
        data: nextNumber,
        mutate: mutateNextNumber
    } = useSWR<NextNumber>("/data/accounting/documents/next-number/" + (year ?? new Date().getFullYear()));

    const sortedDocuments = useMemo(() => {
        return documents ? documents.sort((a, b) => parseISO(a.date).getTime() - parseISO(b.date).getTime() || (a.documentNumber ?? 0) - (b.documentNumber ?? 0)) : [];
    }, [documents]);

    const [selectedDocument, setSelectedDocument] = useState<AccountingDocument | null>(null);
    const [editing, setEditing] = useState<boolean>(false);
    const [displayDocumentNumber, setDisplayDocumentNumber] = useState<boolean>(false);

    const {register, setValue, handleSubmit, formState: {errors}} = useForm();

    async function setYear(delta: number) {
        let nextYear = parseInt(year);
        nextYear += delta;

        navigate("/accounting/documents/" + nextYear);
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
        setValue("category", "");
        setDisplayDocumentNumber(true);
        setEditing(true);
    }

    async function onSaveDocument(data: any) {
        data.amount = parseFloat(data.amount);
        
        if (data.type === "expense") {
            data.amount *= -1;
        }

        if (data.documentNumberChoice === "noGenerate") {
            data.documentNumber = null;
        }
        
        data.date = formatISO(parse(data.date, "dd/MM/yyyy", new Date()), {representation: "date"} );

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
        setValue("category", document.category);

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
    
    const titleSuggestions = useMemo<string[]>(() => {
        return (documents?.map((document) => document.title).filter(onlyUnique) ?? []) as string[];
    }, [documents]);
    
    const categorySuggestions = useMemo<string[]>(() => {
        return (documents?.map((document) => document.category).filter(onlyUnique) ?? []) as string[];
    }, [documents]);

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
                                <InputTextAutoComplete
                                    label={t("documents.headers.title")}
                                    name={"title"}
                                    type={"text"}
                                    required={true}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.title}
                                    suggestions={titleSuggestions}
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
                            <div className={"col-span-2"}>
                                <InputTextAutoComplete
                                    label={t("documents.headers.category")}
                                    name={"category"}
                                    type={"text"}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.title}
                                    suggestions={categorySuggestions}
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
                    {year}
                </h1>

                <div className={"col-start-8 1 w-8 m-auto text-primary"}
                     onClick={() => setYear(1)}>
                    <ArrowSmallRightIcon/>
                </div>
            </div>

            <div className={"grid grid-cols-12 gap-2 mt-4"}>
                <div className={"font-bold col-span-7"}>{t("documents.headers.title")}</div>
                <div className={"font-bold col-span-1"}>{t("documents.headers.type")}</div>
                <div className={"font-bold col-span-1"}>{t("documents.headers.category")}</div>
                <div className={"font-bold col-span-1 text-center"}>{t("documents.headers.reference")}</div>
                <div className={"font-bold col-span-1 text-center"}>{t("documents.headers.number")}</div>
                <div className={"font-bold col-span-1 text-right"}>{t("documents.headers.amount")}</div>
            </div>

            <>
                {documents?.length === 0 && <div className={"text-center p-3 italic"}>
                    {t("documents.noDocuments")}
                </div>}
            </>
            
            <>
                {sortedDocuments?.map((document) => <div key={document.id}
                                                         className={"grid grid-cols-12 gap-2 hover:bg-gray-100" + (document.amount < 0 ? " text-red-700" : " text-green-700")}>
                    <div className={"col-span-7 flex"}>
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
                        </div>
                    </div>
                    <div className={"col-span-1"}>{getDocumentType(document)}</div>
                    <div className={"col-span-1"}>{document.category}</div>
                    <div className={"col-span-1 text-center"}>{document.reference}</div>
                    <div className={"col-span-1 text-center"}>{document.documentNumber}</div>
                    <div className={"col-span-1 text-right"}>{formatAmount(document.amount)} &euro;</div>
                </div>)}
            </>

        </Section>
    </ContentContainer>
}

export default AccountingDocuments;