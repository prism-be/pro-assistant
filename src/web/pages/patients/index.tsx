import styles from '../../styles/pages/patients.module.scss';

import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import InputDate from "../../components/forms/InputDate";
import Button from "../../components/forms/Button";
import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {useEffect, useState} from "react";
import {useRouter} from "next/router";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import {postData} from "../../lib/ajaxHelper";
import {Patient} from "../../lib/contracts";
import Section from "../../components/design/Section";

const Patients: NextPage = () => {

    const {t} = useTranslation('common');
    const router = useRouter();

    const [patients, setPatients] = useState<Patient[] | null>(null);

    const schema = yup.object({}).required();

    const {register, handleSubmit, formState: {errors}, setValue, setFocus, reset} = useForm({resolver: yupResolver(schema)});

    useEffect(() => {
        const sessionSearchPatients = sessionStorage.getItem('patients/search-patients');
        if (sessionSearchPatients) {
            const data = JSON.parse(sessionSearchPatients);
            Object.getOwnPropertyNames(data).forEach(field => {
                setValue(field, data[field]);
            })
            onSubmit(data);
        }
        setFocus('lastName');
    }, [setFocus])

    const onSubmit = async (data: any) => {
        sessionStorage.setItem('patients/search-patients', JSON.stringify(data));
        const result = await postData<Patient[]>("/patients", data);
        setPatients(result);
    }

    const navigate = async (id: string) => {
        await router.push('/patients/' + id);
    }

    const resetSearch = () => {
        sessionStorage.removeItem('patients/search-patients');
        reset();
        setPatients(null);
        setFocus('lastName');
    }

    const [isNewPressed, isNewPressedEvent] = useKeyboardJs("alt + n")
    useEffect(() => {
        if (isNewPressed) {
            isNewPressedEvent?.preventDefault();
            router.push("/patients/000000000000000000000000");
        }
    }, [isNewPressed, router])

    return <ContentContainer>
        <>
            <Section>
                <>
                    <h1 className={styles.searchTitle}>{t("pages.patients.title")}</h1>
                    <form className={styles.searchPanel} onSubmit={handleSubmit(onSubmit)}>
                        <div className={styles.searchField}>
                            <InputText name="lastName" label={t("fields.lastName")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputText name="firstName" label={t("fields.firstName")} type="text" required={false} register={register} setValue={setValue} error={errors.firstName} autoCapitalize={true}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputText name="phoneNumber" label={t("fields.phoneNumber")} type="text" required={false} register={register} setValue={setValue} error={errors.phoneNumber}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputDate name="birthDate" label={t("fields.birthDate")} type="text" required={false} register={register} setValue={setValue} error={errors.birthDate}/>
                        </div>
                        <div className={styles.resetButton}>
                            <Button text={t("actions.reset")} onClick={resetSearch} secondary={true}/>
                        </div>
                        <div className={styles.newButton}>
                            <Button text={t("actions.new")} onClick={() => router.push("/patients/000000000000000000000000")} secondary={true}/>
                        </div>
                        <div className={styles.searchButton}>
                            <Button submit={true} text={t("actions.search")} onClick={handleSubmit(onSubmit)}/>
                        </div>
                    </form>
                </>
            </Section>

            <Section>
                <>
                    {patients && <>
                        <h2>{t("results.title")}</h2>
                        {patients.length !== 0 && <div className={styles.searchResultsTable}>
                            <div className={styles.searchResultsRow}>
                                <div className={styles.searchResultsHeader}>{t("fields.lastName")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.firstName")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.phoneNumber")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.birthDate")}</div>
                            </div>
                            {patients?.map(patient =>
                                <div className={styles.searchResultsRow + " " + styles.searchResultsRowPatient} key={patient.id} onClick={() => navigate(patient.id)}>
                                    <div className={styles.searchResultsCell}>{patient.lastName}</div>
                                    <div className={styles.searchResultsCell}>{patient.firstName}</div>
                                    <div className={styles.searchResultsCell}>{patient.phoneNumber}</div>
                                    <div className={styles.searchResultsCell}>{patient.birthDate}</div>
                                </div>
                            )}
                        </div>}

                        {patients.length === 0 && <div className={styles.helpText}>{t("results.noResults")}</div>}
                    </>}

                    {!patients && <>
                        <div className={styles.helpText}>{t("results.doSearch")}</div>
                    </>}
                </>
            </Section>
        </>
    </ContentContainer>
}

export default Patients;