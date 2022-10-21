import styles from '../../styles/pages/patient.module.scss'

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import {useMsal} from "@azure/msal-react";
import ContentContainer from "../../components/design/ContentContainer";
import {useRouter} from "next/router";
import useSWR from "swr";
import {getPatient, savePatient} from "../../lib/services/Patients";
import InputText from "../../components/forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect} from "react";
import Button from "../../components/forms/Button";
import {success} from "../../lib/events/alert";

const Patient: NextPage = () => {
    const {t} = useTranslation('patients');
    const {instance, accounts} = useMsal();
    const router = useRouter();
    const {pid} = router.query;

    const loadPatient = async (id: string) => {
        return await getPatient(id, instance, accounts[0]);
    }
    const {data: patient, mutate: mutatePatient} = useSWR(pid, loadPatient);
    
    useEffect(() => {
        if (patient) {
            const d: any = patient;
            Object.getOwnPropertyNames(patient).forEach(field => {
                setValue(field, d[field]);
            })
        }
    }, [patient])

    const {register, handleSubmit, formState: {errors}, setValue } = useForm();

    const onSubmit = async (data: any) => {
        await savePatient(data, instance, accounts[0]);
        await mutatePatient();
        success(t("details.saveSuccess"), { autoClose: true });
    }

    return <ContentContainer>
        <>
            <div className={styles.card}>
                <h1>{t("details.title")} {patient?.lastName} {patient?.firstName}</h1>
                <h2>{t("details.contact")}</h2>
                <form className={styles.contact} onSubmit={handleSubmit(onSubmit)}>
                    <div className={styles.contactField}>
                        <InputText name="lastName" label={t("fields.lastName")} type="text" required={false} register={register} error={errors.lastName}/>
                    </div>
                    <div className={styles.contactField}>
                        <InputText name="firstName" label={t("fields.firstName")} type="text" required={false} register={register} error={errors.firstName}/>
                    </div>
                    <div className={styles.contactField}>
                        <InputText name="email" label={t("fields.email")} type="text" required={false} register={register} error={errors.email}/>
                    </div>
                    <div className={styles.contactFieldSmall}>
                        <InputText name="phoneNumber" label={t("fields.phoneNumber")} type="text" required={false} register={register} error={errors.phoneNumber}/>
                    </div>
                    <div className={styles.contactFieldSmall}>
                        <InputText name="birthDate" label={t("fields.birthDate")} type="text" required={false} register={register} error={errors.birthDate}/>
                    </div>
                    <div className={styles.contactFieldWide}>
                        <InputText name="street" label={t("fields.street")} type="text" required={false} register={register} error={errors.street}/>
                    </div>
                    <div className={styles.contactFieldSmall}>
                        <InputText name="number" label={t("fields.number")} type="text" required={false} register={register} error={errors.number}/>
                    </div>
                    <div className={styles.contactFieldSmall}>
                        <InputText name="zipCode" label={t("fields.zipCode")} type="text" required={false} register={register} error={errors.zipCode}/>
                    </div>
                    <div className={styles.contactField}>
                        <InputText name="city" label={t("fields.city")} type="text" required={false} register={register} error={errors.city}/>
                    </div>
                    <div className={styles.contactFieldSmall}>
                        <InputText name="country" label={t("fields.country")} type="text" required={false} register={register} error={errors.country}/>
                    </div>
                    <div className={styles.saveButton}>
                        <Button text={t("details.save")} onClick={handleSubmit(onSubmit)} secondary={true}/>
                    </div>
                </form>
            </div>
        </>
    </ContentContainer>
}

export default Patient;