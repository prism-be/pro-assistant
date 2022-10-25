import styles from '../styles/pages/configuration.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {useMsal} from "@azure/msal-react";
import {getTariffs, upsertTariff} from "../lib/services/tariffs";
import useSWR from "swr";
import {useState} from "react";
import InputText from "../components/forms/InputText";
import {useForm} from "react-hook-form";
import Button from "../components/forms/Button";
import {Popup} from '../components/Pops';
import {useKeyPressEvent} from "react-use";
import {alertSuccess} from "../lib/events/alert";


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

    const onSaveTariff = async (data: any) => {
        data.price = parseFloat(data.price);
        await upsertTariff(data, instance, accounts[0]);
        setEditing(false);
        alertSuccess(t("common:alerts.saveSuccess"), { });
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
                    <Button className={styles.tariffEditionGridButtonCancel} text={t("common:actions.cancel")} onClick={() => setEditing(false)} secondary={true}/>
                    <Button className={styles.tariffEditionGridButtonSave} text={t("common:actions.save")} onClick={handleSubmit(onSaveTariff)}/>
                </div>
            </form>
        </Popup>}

        <div>
            {tariffs?.map(tariff =>
                <div key={tariff.id} className={styles.tariffGrid}>
                    <div>
                        {tariff.name}
                    </div>
                    <div>
                        {tariff.price} &euro;
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