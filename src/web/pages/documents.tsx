import grid from '../styles/grid.module.scss';
import table from '../styles/table.module.scss';
import styles from '../styles/styles.module.scss';


import React from 'react';
import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import Section from "../components/design/Section";
import useSWR from "swr";
import {DocumentConfiguration} from "../lib/contracts";
import {useState} from "react";
import Button from "../components/forms/Button";
import {useForm} from "react-hook-form";
import InputText from "../components/forms/InputText";
import TextArea from "../components/forms/TextArea";
import {deleteData, postData} from "../lib/ajaxHelper";
import {Delete, Pencil} from "../components/icons/Icons";

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
                    <header>
                        <h1>{t("title")}</h1>
                        <Button secondary={true} onClick={() => createNew()} text={t("common:actions.new")}/>
                    </header>
                    <div>
                        {documents?.length === 0 && <>
                            <i className={styles.tip}>{t("list.NoElements")}</i>
                        </>}
                        {documents && documents.length > 0 && <>
                            <div className={table.rowAction2}>
                                {documents.map(d => <React.Fragment key={d.id}>
                                    <a className={table.rowAction + " " + styles.iconButton} onClick={() => editDocument(d)}> <Pencil/> </a>
                                    <a className={table.rowAction + " " + styles.iconButton} onClick={() => deleteDocument(d)}> <Delete/> </a>
                                    <div className={table.table10}>{d.name}</div>
                                </React.Fragment>)}
                            </div>
                        </>}
                    </div>
                </>
            </Section>

            {currentDocument && <Section>
                <>
                    <header>
                        <h1>{t("edit.title")}</h1>
                        <Button secondary={true} onClick={() => saveDocument()} text={t("common:actions.save")}/>
                    </header>
                    <div className={grid.container}>
                        <InputText className={grid.extraLarge} label={t("edit.form.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name}/>
                        <InputText className={grid.extraLarge} label={t("edit.form.title")} name={"title"} type={"text"} required={true} register={register} setValue={setValue} error={errors.title}/>
                        <TextArea className={grid.extraLarge + " " + styles.text} label={t("edit.form.body")} name={"body"} required={true} register={register} error={errors.body}/>
                        <div className={grid.extraLarge}>
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