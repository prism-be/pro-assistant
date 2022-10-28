import styles from '../styles/pages/configuration.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {useMsal} from "@azure/msal-react";
import {getTariffs, Tariff, upsertTariff} from "../lib/services/tariffs";
import useSWR from "swr";
import {useState} from "react";
import InputText from "../components/forms/InputText";
import {useForm} from "react-hook-form";
import Button from "../components/forms/Button";
import {Popup} from '../components/Pops';
import {useKeyPressEvent} from "react-use";
import {alertSuccess} from "../lib/events/alert";
import {Pencil} from "../components/icons/Icons";


const Tariffs = () => {
    const {instance, accounts} = useMsal();
    const {t} = useTranslation("configuration");
    const {register, handleSubmit, formState: {errors}, setValue} = useForm();
    const [editing, setEditing] = useState<boolean>(false);

    useKeyPressEvent('Escape', () => {
        setEditing(false);
    })

    const loadTariffs = async () => {
        return await getTariffs(instance, accounts[0]);
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
        await upsertTariff(data, instance, accounts[0]);
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

const Configuration: NextPage = () => {
    return <ContentContainer>
        <Tariffs/>
    </ContentContainer>
}

export default Configuration;