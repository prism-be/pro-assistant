import React, {useState} from 'react';
import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import Section from "../components/design/Section";
import useSWR from "swr";
import {DocumentConfiguration} from "../lib/contracts";
import {useForm} from "react-hook-form";
import InputText from "../components/forms/InputText";
import TextArea from "../components/forms/TextArea";
import {deleteData, postData} from "../lib/ajaxHelper";
import {PencilSquareIcon, TrashIcon} from '@heroicons/react/24/outline';
import {HeaderWithAction} from "../components/design/HeaderWithAction";

const Documents: NextPage = () => {

    const {t} = useTranslation("documents");
    const {data: documents, mutate: mutateDocuments} = useSWR<DocumentConfiguration[] | null>('/documents-configuration');
    const [currentDocument, setCurrentDocument] = useState<DocumentConfiguration | null>(null);
    const {register, getValues, formState: {errors}, setValue} = useForm();

    const createNew = () => {
        setCurrentDocument({id: ""});
        setValue("name", "");
        setValue("title", "");
        setValue("body", "");
    }

    const editDocument = async (document: DocumentConfiguration) => {
        setCurrentDocument(document);
        setValue("name", document.name);
        setValue("title", document.title);
        setValue("body", document.body);
    }

    const deleteDocument = async (document: DocumentConfiguration) => {
        if (confirm(t("edit.confirmDelete") + document.name)) {
            await deleteData("/documents-configuration/" + document.id);
            await mutateDocuments();
        }
    }

    const saveDocument = async () => {
        const data = getValues();
        data.id = currentDocument?.id ?? '';
        await postData("/documents-configuration", data);
        await mutateDocuments();
        setCurrentDocument(null);
    }

    return <ContentContainer>
        <>
            <Section>
                <>
                    <HeaderWithAction title={t("title")} action={() => createNew()} actionText={t("common:actions.new")}/>

                    <div>
                        {documents?.length === 0 && <>
                            <i className={"text-center italic p-4"}>{t("list.NoElements")}</i>
                        </>}
                        {documents && documents.length > 0 && <>
                            {documents.map(d => <div className={"flex"} key={d.id}>
                                <a className={"w-6 cursor-pointer"} onClick={() => editDocument(d)}> <PencilSquareIcon/> </a>
                                <a className={"w-6 ml-2 cursor-pointer"} onClick={() => deleteDocument(d)}> <TrashIcon/> </a>
                                <div className={"pl-2"}>{d.name}</div>
                            </div>)}
                        </>}
                    </div>
                </>
            </Section>

            {currentDocument && <Section>
                <>
                    <HeaderWithAction title={t("edit.title")} action={() => saveDocument()} actionText={t("common:actions.save")}/>

                    <div className={""}>
                        <InputText className={""} label={t("edit.form.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name}/>
                        <InputText className={""} label={t("edit.form.title")} name={"title"} type={"text"} required={true} register={register} setValue={setValue} error={errors.title}/>
                        <TextArea className={""} label={t("edit.form.body")} name={"body"} required={true} register={register} error={errors.body}/>
                        <div className={""}>
                            <h3>{t("edit.help.title")}</h3>
                            <div>{t("edit.help.help")}</div>
                            <ul>
                                <li><b>{"{{name}}"}</b> : {t("edit.help.name")}</li>
                                <li><b>{"{{contactName}}"}</b> : {t("edit.help.contactName")}</li>
                                <li><b>{"{{price}}"}</b> : {t("edit.help.price")}</li>
                                <li><b>{"{{appointmentType}}"}</b> : {t("edit.help.appointmentType")}</li>
                                <li><b>{"{{appointmentDate}}"}</b> : {t("edit.help.appointmentDate")}</li>
                                <li><b>{"{{appointmentHour}}"}</b> : {t("edit.help.appointmentHour")}</li>
                                <li><b>{"{{paymentDate}}"}</b> : {t("edit.help.paymentDate")}</li>
                                <li><b>{"{{paymentMode}}"}</b> : {t("edit.help.paymentMode")}</li>
                            </ul>
                        </div>
                    </div>
                </>
            </Section>}
        </>
    </ContentContainer>
}

export default Documents;