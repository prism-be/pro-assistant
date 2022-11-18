import styles from '../styles/pages/documents.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import Section from "../components/design/Section";
import useSWR from "swr";
import {IDocument} from "../lib/contracts";
import {useState} from "react";
import Button from "../components/forms/Button";
import {useForm} from "react-hook-form";
import InputText from "../components/forms/InputText";
import TextArea from "../components/forms/TextArea";
import {deleteData, getData, postData} from "../lib/ajaxHelper";
import {Delete, Pencil} from "../components/icons/Icons";
import React from 'react';

const Documents: NextPage = () => {

    const {t} = useTranslation("documents");
    const {data: documents, mutate: mutateDocuments} = useSWR<IDocument[] | null>('/documents');
    const [currentDocument, setCurrentDocument] = useState<IDocument | null>(null);
    const {register, getValues, formState: {errors}, setValue} = useForm();

    const createNew = () => {
        setCurrentDocument({});
    }
    
    const editDocument = async (document: IDocument) => {
        setCurrentDocument(document);
        setValue("name", document.name);
        setValue("title", document.title);
        setValue("body", document.body);
    }
    
    const deleteDocument = async (document: IDocument) => {
        if (confirm(t("edit.confirmDelete")  + document.name))
        {
            await deleteData("/documents/" + document.id);
            await mutateDocuments();
        }
    }

    const saveDocument = async () => {
        const data = getValues();
        data.id = currentDocument?.id ?? '';
        await postData("/documents", data);
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
                            <div className={styles.edition}>
                                {documents.map(d => <React.Fragment key={d.id}>
                                    <a className={styles.editionPencil} onClick={() => editDocument(d)}> <Pencil/> </a>
                                    <a className={styles.editionTrash} onClick={() => deleteDocument(d)}> <Delete/> </a>
                                    <div className={styles.editionName}>{d.name}</div>
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
                    <div>
                        <InputText label={t("edit.form.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name}/>
                        <InputText label={t("edit.form.title")} name={"title"} type={"text"} required={true} register={register} setValue={setValue} error={errors.title}/>
                        <TextArea className={styles.body} label={t("edit.form.body")} name={"body"} required={true} register={register} error={errors.body}/>
                    </div>
                </>
            </Section>}
        </>
    </ContentContainer>
}

export default Documents;