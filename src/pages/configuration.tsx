import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import useSWR from "swr";
import {useEffect, useState} from "react";
import InputText from "../components/forms/InputText";
import {useForm} from "react-hook-form";
import Button from "../components/forms/Button";
import {useKeyPressEvent} from "react-use";
import TextArea from "../components/forms/TextArea";
import InputImage from "../components/forms/InputImage";
import Section from "../components/design/Section";
import InputColor from "../components/forms/InputColor";
import {PencilSquareIcon} from '@heroicons/react/24/outline';
import {Setting, Tariff} from "@/libs/models";
import {postData, putData} from "@/libs/http";
import {alertSuccess} from "@/modules/events/alert";
import {HeaderWithAction} from "@/components/design/HeaderWithAction";
import {Popup} from "@/components/Pops";

const Tariffs = () => {
    const {t} = useTranslation("configuration");
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const [editing, setEditing] = useState<boolean>(false);

    useKeyPressEvent('Escape', () => {
        setEditing(false);
    })

    const {data: tariffs, mutate: mutateTariffs} = useSWR<Tariff[]>("/data/tariffs/list");

    const addTariff = () => {
        setValue("id", "");
        setValue("name", "");
        setValue("price", 0);
        setValue("backgroundColor", "#31859c");
        setEditing(true);
    }

    const editTariff = (tariff: Tariff) => {
        setValue("id", tariff._id);
        setValue("name", tariff.name);
        setValue("price", tariff.price.toFixed(2));
        setValue("defaultDuration", tariff.defaultDuration);
        setValue("backgroundColor", tariff.backgroundColor ?? "#31859c");
        setEditing(true);
    }

    const onSaveTariff = async (data: any) => {
        data.price = parseFloat(data.price);
        data.defaultDuration = parseInt(data.defaultDuration);
        if (data.id && data.id.length > 0) {
            data._id = data.id;
            delete data.id;
            await postData("/data/tariffs/update", data);
        }
        else {
            delete data.id;
            await postData("/data/tariffs/insert", data);
        }
        setEditing(false);
        alertSuccess(t("common:alerts.saveSuccess"), {});
        await mutateTariffs();
    }

    return <Section>
        <>
            <HeaderWithAction title={t('tariffs.title')} action={() => addTariff()} actionText={t("common:actions.add")}/>

            {editing && <Popup>
                <form>
                    <div className={"grid grid-cols-2 gap-2"}>
                        <div className={"col-span-2"}>
                            <InputText label={t("tariffs.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name}/>
                        </div>
                        <div className={"col-span-2"}>
                            <InputText label={t("tariffs.price")} name={"price"} type={"text"} required={true} register={register} setValue={setValue} error={errors.price}/>
                        </div>
                        <div className={"col-span-2"}>
                            <InputText label={t("tariffs.defaultDuration")} name={"defaultDuration"} type={"number"} required={true} register={register} setValue={setValue} error={errors.defaultDuration}/>
                        </div>
                        <div className={"col-span-2"}>
                            <InputColor label={t("tariffs.color")} name={"backgroundColor"} setValue={setValue} error={errors.backgroundColor} initialColor={getValues()["backgroundColor"]}/>
                        </div>
                        <Button text={t("common:actions.cancel")} onClick={() => setEditing(false)} secondary={true}/>
                        <Button text={t("common:actions.save")} onClick={handleSubmit(onSaveTariff)}/>
                    </div>
                </form>
            </Popup>}

            <div>
                {tariffs?.map(tariff =>
                    <div key={tariff._id} className={"flex"}>
                        <div className={"w-6"}>
                            <a className={"cursor-pointer"} onClick={() => editTariff(tariff)}>
                                <PencilSquareIcon/>
                            </a>
                        </div>
                        <div className={"pl-2"}>
                            {tariff.name} - {tariff.defaultDuration}m
                        </div>
                        <div className={"pl-2"}>
                            ({tariff.price.toFixed(2)} &euro;)
                        </div>

                    </div>)}

            </div>
        </>
    </Section>
}

const Documents = () => {
    const {t} = useTranslation("configuration");
    const {register, setValue, getValues} = useForm();
    const {data: settings, mutate: mutateSettings} = useSWR<Setting[]>("/data/settings/list");
    const [logo, setLogo] = useState<string>();
    const [signature, setSignature] = useState<string>();
    const [accentuateColor, setAccentuateColor] = useState<string>();

    useEffect(() => {
        if (settings) {
            setValue('name', findSetting("document-header-name")?.value);
            setValue('address', findSetting("document-header-address")?.value);
            setValue('logo', findSetting("document-header-logo")?.value);
            setValue('yourName', findSetting("document-header-your-name")?.value);
            setValue('yourCity', findSetting("document-header-your-city")?.value);
            setValue('signature', findSetting("document-header-signature")?.value);
            setValue('accentuateColor', findSetting("document-header-accentuate-color")?.value);
            setAccentuateColor(findSetting("document-header-accentuate-color")?.value);
            setLogo(findSetting("document-header-logo")?.value);
            setSignature(findSetting("document-header-signature")?.value);
        }
    }, [settings, setValue]);

    function findSetting(id: string) {
        return settings?.find(s => s._id === id);
    }

    const saveDocumentHeaders = async () => {
        const data = getValues();

        let settings: Setting[] = [];
        settings.push({_id: "document-header-name", value: data.name});
        settings.push({_id: "document-header-address", value: data.address});
        settings.push({_id: "document-header-logo", value: data.logo});
        settings.push({_id: "document-header-your-name", value: data.yourName});
        settings.push({_id: "document-header-your-city", value: data.yourCity});
        settings.push({_id: "document-header-signature", value: data.signature});
        settings.push({_id: "document-header-accentuate-color", value: data.accentuateColor});
        settings.push({_id: "document-header-logo", value: data.logo});
        settings.push({_id: "document-header-signature", value: data.signature});

        await putData("/data/settings/update-many", settings);
        alertSuccess(t("documents.header.saveSuccess"));
        await mutateSettings();
    }

    return <Section>
        <>
            <HeaderWithAction title={t("documents.header.title")} action={() => saveDocumentHeaders()} actionText={t("common:actions.save")}/>
            
            <div className={"grid grid-cols-4 gap-2"}>
                <InputText className={"col-span-4 md:col-span-2"} label={t("documents.header.name")} name={"name"} type={"text"} register={register} setValue={setValue}/>
                <TextArea className={"col-span-4 md:col-span-2"} label={t("documents.header.address")} name={"address"} register={register}/>
                <InputImage className={"col-span-4 md:col-span-2"} label={t("documents.header.logo")} name={"logo"} register={register} setValue={setValue} initialPreview={logo}/>
                <InputImage className={"col-span-4 md:col-span-2"} label={t("documents.header.signature")} name={"signature"} register={register} setValue={setValue} initialPreview={signature}/>
                <InputText className={"col-span-4 md:col-span-2"} label={t("documents.header.yourName")} name={"yourName"} type={"text"} register={register} setValue={setValue}/>
                <InputText className={"col-span-4 md:col-span-2"} label={t("documents.header.yourCity")} name={"yourCity"} type={"text"} register={register} setValue={setValue}/>
                <InputColor className={"col-span-4 md:col-span-2"} label={t("documents.header.accentuateColor")} name={"accentuateColor"} setValue={setValue} initialColor={accentuateColor}/>
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