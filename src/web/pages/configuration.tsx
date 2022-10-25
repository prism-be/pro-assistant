import styles from '../styles/pages/configuration.module.scss';

import {NextPage} from "next";
import ContentContainer from "../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {useMsal} from "@azure/msal-react";
import {getTariffs, Tariff} from "../lib/services/tariffs";
import useSWR from "swr";
import {useEffect, useState} from "react";
import InputText from "../components/forms/InputText";
import {Plus} from "../components/icons/Plus";
import { v4 as uuidv4 } from 'uuid';
import {useForm} from "react-hook-form";
import {Save} from "../components/icons/Save";

const Configuration: NextPage = () => {

    const {instance, accounts} = useMsal();
    const {t} = useTranslation("configuration");
    const {register, handleSubmit, formState: {errors}, setValue, getValues } = useForm();
    const [currentTariffEdition, setCurrentTariffEdition] = useState<Tariff>();

    const loadTariffs = async (id: string) => {
        return await getTariffs(instance, accounts[0]);
    }
    const {data: tariffs, mutate: mutatePatient} = useSWR("/tariffs", loadTariffs);
    
    const addTariff = () => {
        setCurrentTariffEdition({
            id: uuidv4(),
            name: "",
            price: 0
        });
    }
    

    return <ContentContainer>
        <section className={styles.section}>
            <h1>{t("tariffs.title")}</h1>

            <div className={styles.tariffAdd}>
                <a onClick={() => addTariff()}>
                    <Plus />
                </a>
            </div>
            
            <form>
                {currentTariffEdition && <div className={styles.tariffGrid}>
                    <div>
                        <InputText label={t("tariffs.name")} name={"name"} type={"text"} required={true} register={register} setValue={setValue} error={errors.name} />
                    </div>
                    <div>
                        <InputText label={t("tariffs.price")} name={"price"} type={"text"} required={true} register={register} setValue={setValue} error={errors.price} />
                    </div>
                    <div>
                        <a>
                            <Save />
                        </a>
                    </div>
                </div>}
            </form>
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
    </ContentContainer>
}

export default Configuration;