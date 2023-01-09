import styles from '../../styles/pages/patient.module.scss'

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";
import {useRouter} from "next/router";
import useSWR from "swr";
import InputText from "../../components/forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect} from "react";
import Button from "../../components/forms/Button";
import {alertSuccess} from "../../lib/events/alert";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import {Back} from "../../components/icons/Back";
import InputDate from "../../components/forms/InputDate";
import {getData, postData} from "../../lib/ajaxHelper";
import {Patient, UpsertResult} from "../../lib/contracts";
import Section from "../../components/design/Section";
import {PatientAppointments} from "../../components/patients/PatientAppointments";

const Patient: NextPage = () => {
    const {t} = useTranslation('common');
    const router = useRouter();
    const {pid} = router.query;

    const loadPatient = async (route: string) => {

        if (route === "/patient/000000000000000000000000") {
            return null;
        }

        return await getData<Patient>(route);
    }

    const {data: patient, mutate: mutatePatient} = useSWR("/patient/" + pid, loadPatient);
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();

    useEffect(() => {
        if (patient) {
            const d: any = patient;
            Object.getOwnPropertyNames(patient).forEach(field => {
                setValue(field, d[field]);
            })
        }
    }, [patient, setValue])


    const savePatientForm = async (data: any) => {
        if (pid === '000000000000000000000000') {
            data.id = '';
            const newPid = await postData<UpsertResult>("/patient", data);
            alertSuccess(t("details.saveSuccess"), {autoClose: true});
            await router.push("/patients/" + newPid?.id);
            return;
        }

        await postData("/patient", data);
        await mutatePatient();
        alertSuccess(t("pages.patients.details.saveSuccess"), {autoClose: true});
    }

    const onSavePatientSubmit = async (data: any) => {
        await savePatientForm(data);
    }

    const [isSavePressed, isSaveEvent] = useKeyboardJs("ctrl + s")
    useEffect(() => {
        if (isSavePressed) {
            isSaveEvent?.preventDefault();
            const data = getValues();
            savePatientForm(data);
        }
    }, [isSavePressed, getValues])


    return <ContentContainer>
        <Section>
            <div className={styles.cardTitle}>
                <a onClick={() => router.push("/patients")}>
                    <Back/>
                </a>
                <h1>{t("pages.patients.details.title")} {patient?.lastName} {patient?.firstName}</h1>
            </div>
            <h2>{t("pages.patients.details.contact")}</h2>
            <form className={styles.contact} onSubmit={handleSubmit(onSavePatientSubmit)}>
                <div className={styles.contactFieldExtraSmall}>
                    <InputText name="title" label={t("fields.title")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldAverageSmall}>
                    <InputText name="lastName" label={t("fields.lastName")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldAverageSmall}>
                    <InputText name="firstName" label={t("fields.firstName")} type="text" required={false} register={register} setValue={setValue} error={errors.firstName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactField}>
                    <InputText name="email" label={t("fields.email")} type="text" required={false} register={register} setValue={setValue} error={errors.email}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="phoneNumber" label={t("fields.phoneNumber")} type="text" required={false} register={register} setValue={setValue} error={errors.phoneNumber}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputDate name="birthDate" label={t("fields.birthDate")} type="text" required={false} register={register} setValue={setValue} error={errors.birthDate}/>
                </div>
                <div className={styles.contactFieldWide}>
                    <InputText name="street" label={t("fields.street")} type="text" required={false} register={register} setValue={setValue} error={errors.street} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="number" label={t("fields.number")} type="text" required={false} register={register} setValue={setValue} error={errors.number}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="zipCode" label={t("fields.zipCode")} type="text" required={false} register={register} setValue={setValue} error={errors.zipCode}/>
                </div>
                <div className={styles.contactField}>
                    <InputText name="city" label={t("fields.city")} type="text" required={false} register={register} setValue={setValue} error={errors.city} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="country" label={t("fields.country")} type="text" required={false} register={register} setValue={setValue} error={errors.country} autoCapitalize={true}/>
                </div>
                <div className={styles.saveButton}>
                    <Button text={t("actions.save")} onClick={handleSubmit(onSavePatientSubmit)} secondary={true}/>
                </div>
            </form>
        </Section>
        <PatientAppointments patientId={pid as string}/>
    </ContentContainer>
}

export default Patient;