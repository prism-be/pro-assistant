import styles from '../styles/pages/configuration.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {getTariffs, Tariff, upsertTariff} from "../lib/services/tariffs";
import useSWR from "swr";
import {useEffect, useState} from "react";
import InputText from "../components/forms/InputText";
import {useForm} from "react-hook-form";
import Button from "../components/forms/Button";
import {Popup} from '../components/Pops';
import {useKeyPressEvent} from "react-use";
import {alertSuccess} from "../lib/events/alert";
import {Pencil} from "../components/icons/Icons";
import {getSettings, saveSettings} from "../lib/services/settings";
import TextArea from "../components/forms/TextArea";
import InputImage from "../components/forms/InputImage";


const Tariffs = () => {
    const {t} = useTranslation("configuration");
    const {register, handleSubmit, formState: {errors}, setValue} = useForm();
    const [editing, setEditing] = useState<boolean>(false);

    useKeyPressEvent('Escape', () => {
        setEditing(false);
    })

    const loadTariffs = async () => {
        return await getTariffs();
    }
    const {data: tariffs, mutate: mutateTariffs} = useSWR("/tariffs", loadTariffs);

    const addTariff = () => {
        setValue("id", "");
        setValue("name", "");
        setValue("price", 0);
        setEditing(true);
    }

    const editTariff = (tariff: Tariff) => {
        setValue("id", tariff.id);
        setValue("name", tariff.name);
        setValue("price", tariff.price.toFixed(2));
        setValue("defaultDuration", tariff.defaultDuration);
        setEditing(true);
    }

    const onSaveTariff = async (data: any) => {
        data.price = parseFloat(data.price);
        data.defaultDuration = parseInt(data.defaultDuration);
        await upsertTariff(data);
        setEditing(false);
        alertSuccess(t("common:alerts.saveSuccess"), {});
        await mutateTariffs();
    }

    return <section className={styles.section}>
        <header>
            <h2>{t("tariffs.title")}</h2>
            <Button text={t("common:actions.add")} onClick={() => addTariff()} secondary={true}></Button>
        </header>

        {editing && <Popup>
            <form>
                <div className={styles.tariffEditionGrid}>
                    <div className={styles.tariffEditionGridField}>
                        <InputText label={t("tariffs.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name}/>
                    </div>
                    <div className={styles.tariffEditionGridField}>
                        <InputText label={t("tariffs.price")} name={"price"} type={"text"} required={true} register={register} setValue={setValue} error={errors.price}/>
                    </div>
                    <div className={styles.tariffEditionGridField}>
                        <InputText label={t("tariffs.defaultDuration")} name={"defaultDuration"} type={"number"} required={true} register={register} setValue={setValue} error={errors.defaultDuration}/>
                    </div>
                    <Button className={styles.tariffEditionGridButtonCancel} text={t("common:actions.cancel")} onClick={() => setEditing(false)} secondary={true}/>
                    <Button className={styles.tariffEditionGridButtonSave} text={t("common:actions.save")} onClick={handleSubmit(onSaveTariff)}/>
                </div>
            </form>
        </Popup>}

        <div className={styles.tariffGrid}>
            {tariffs?.map(tariff =>
                <div key={tariff.id} className={styles.tariffGridRow}>
                    <div>
                        {tariff.name} - {tariff.defaultDuration}m
                    </div>
                    <div>
                        {tariff.price.toFixed(2)} &euro;
                    </div>
                    <div>
                        <a onClick={() => editTariff(tariff)}>
                            <Pencil/>
                        </a>
                    </div>
                </div>)}

        </div>
    </section>
}

const Documents = () => {
    const {t} = useTranslation("configuration");
    const {register, setValue, getValues} = useForm();
    const headers = useSWR('documents-headers', getSettings) as any;
    const [logo, setLogo] = useState<string>();
    const [signature, setSignature] = useState<string>();

    useEffect(() => {
        if (headers.data) {
            setValue('name', headers.data.name)
            setValue('address', headers.data.address)
            setValue('logo', headers.data.logo)
            setValue('yourName', headers.data.yourName)
            setValue('yourCity', headers.data.yourCity)
            setValue('signature', headers.data.signature)
            setLogo(headers.data.logo);
            setSignature(headers.data.signature);
        }
    }, [headers]);

    const saveDocumentHeaders = async () => {
        const data = getValues();
        await saveSettings("documents-headers", data);
        alertSuccess(t("documents.header.saveSuccess"));
    }

    return <section className={styles.section}>
        <header>
            <h2>{t("documents.header.title")}</h2>
            <Button text={t("common:actions.save")} onClick={() => saveDocumentHeaders()} secondary={true}></Button>
        </header>
        <div className={styles.keyValueForm}>
            <InputText label={t("documents.header.name")} name={"name"} type={"text"} register={register} setValue={setValue}/>
            <TextArea label={t("documents.header.address")} name={"address"} register={register}/>
            <InputImage label={t("documents.header.logo")} name={"logo"} register={register} setValue={setValue} initialPreview={logo}/>
            <InputImage label={t("documents.header.signature")} name={"signature"} register={register} setValue={setValue} initialPreview={signature}/>
            <InputText label={t("documents.header.yourName")} name={"yourName"} type={"text"} register={register} setValue={setValue}/>
            <InputText label={t("documents.header.yourCity")} name={"yourCity"} type={"text"} register={register} setValue={setValue}/>
        </div>
    </section>
}

const DocumentReceipt = () => {

    const {t} = useTranslation("configuration");
    const {register, setValue, getValues} = useForm();
    const document = useSWR('document-receipt', getSettings) as any;

    useEffect(() => {
        if (document.data) {
            setValue('title', document.data.title);
            setValue('content', document.data.content);
        } else {
            setValue('title', t("documents.receipt.default.title"));
            setValue('content', t("documents.receipt.default.content"));
        }
    }, [document]);

    const saveReceipt = async () => {
        const data = getValues();
        await saveSettings("document-receipt", data);
        alertSuccess(t("documents.receipt.saveSuccess"));
    }

    return <section className={styles.section}>
        <header>
            <h2>{t("documents.receipt.title")}</h2>
            <Button text={t("common:actions.save")} onClick={() => saveReceipt()} secondary={true}></Button>
        </header>
        <div className={styles.keyValueForm}>
            <InputText label={t("documents.receipt.document.title")} name={"title"} type={"text"} register={register} setValue={setValue}/>
            <TextArea className={styles.bigText} label={t("documents.receipt.document.content")} name={"content"} register={register}/>
        </div>
    </section>
}

const Configuration: NextPage = () => {
    return <ContentContainer>
        <>
            <Tariffs/>
            <Documents/>
            <DocumentReceipt/>
        </>
    </ContentContainer>
}

export default Configuration;