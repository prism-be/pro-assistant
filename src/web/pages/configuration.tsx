import styles from '../styles/pages/configuration.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import useSWR from "swr";
import {useEffect, useState} from "react";
import InputText from "../components/forms/InputText";
import {useForm} from "react-hook-form";
import Button from "../components/forms/Button";
import {Popup} from '../components/Pops';
import {useKeyPressEvent} from "react-use";
import {alertSuccess} from "../lib/events/alert";
import {Pencil} from "../components/icons/Icons";
import TextArea from "../components/forms/TextArea";
import InputImage from "../components/forms/InputImage";
import {Tariff} from "../lib/contracts";
import {getData, postData} from "../lib/ajaxHelper";
import Section from "../components/design/Section";
import InputColor from "../components/forms/InputColor";

const Tariffs = () => {
    const {t} = useTranslation("configuration");
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const [editing, setEditing] = useState<boolean>(false);

    useKeyPressEvent('Escape', () => {
        setEditing(false);
    })

    const {data: tariffs, mutate: mutateTariffs} = useSWR<Tariff[]>("/tariffs");

    const addTariff = () => {
        setValue("id", "");
        setValue("name", "");
        setValue("price", 0);
        setValue("backgroundColor", "#31859c");
        setEditing(true);
    }

    const editTariff = (tariff: Tariff) => {
        setValue("id", tariff.id);
        setValue("name", tariff.name);
        setValue("price", tariff.price.toFixed(2));
        setValue("defaultDuration", tariff.defaultDuration);
        setValue("backgroundColor", tariff.backgroundColor ?? "#31859c");
        setEditing(true);
    }

    const onSaveTariff = async (data: any) => {
        data.price = parseFloat(data.price);
        data.defaultDuration = parseInt(data.defaultDuration);
        await postData("/tariff", data);
        setEditing(false);
        alertSuccess(t("common:alerts.saveSuccess"), {});
        await mutateTariffs();
    }

    return <Section>
        <>
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
                        <div className={styles.tariffEditionGridField}>
                            <InputColor label={t("tariffs.color")} name={"backgroundColor"} setValue={setValue} error={errors.backgroundColor} getValues={getValues}/>
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
        </>
    </Section>
}

const getSetting = async (route: string) => {
    const result = await getData<any>(route);

    if (result == null) {
        return null;
    }

    return JSON.parse(result.value);
}

const Documents = () => {
    const {t} = useTranslation("configuration");
    const {register, setValue, getValues} = useForm();
    const {data: headers, mutate: mutateHeaders} = useSWR("/setting/documents-headers", getSetting) as any;
    const [logo, setLogo] = useState<string>();
    const [signature, setSignature] = useState<string>();

    useEffect(() => {
        if (headers) {
            setValue('name', headers.name)
            setValue('address', headers.address)
            setValue('logo', headers.logo)
            setValue('yourName', headers.yourName)
            setValue('yourCity', headers.yourCity)
            setValue('signature', headers.signature)
            setLogo(headers.logo);
            setSignature(headers.signature);
        }
    }, [headers, setValue]);

    const saveDocumentHeaders = async () => {
        const data = getValues();
        const setting = {
            value: JSON.stringify(data),
            id: "documents-headers"
        }
        await postData("/setting", setting);
        alertSuccess(t("documents.header.saveSuccess"));
        await mutateHeaders();
    }

    return <Section>
        <>
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
        </>
    </Section>
}

const Configuration: NextPage = () => {
    return <ContentContainer>
        <>
            <Tariffs/>
            <Documents/>
        </>
    </ContentContainer>
}

export default Configuration;