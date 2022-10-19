import styles from '../../styles/pages/patients.module.scss';

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import Button from "../../components/forms/Button";
import {PatientSummary, SearchParameter, searchPatients} from "../../lib/services/Patients";
import {useState} from "react";
import {useMsal} from "@azure/msal-react";
import React from 'react';

const Patients: NextPage = () => {

    const {t} = useTranslation('patients');
    const {instance, accounts} = useMsal();

    const [patients, setPatients] = useState<PatientSummary[]>()

    const schema = yup.object({}).required();

    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});

    const onSubmit = async (data: any) => {
        const result = await searchPatients(data, instance, accounts[0]);
        setPatients(result);
    }

    return <ContentContainer>
        <>
            <div className={styles.searchContainer}>
                <h1 className={styles.searchTitle}>{t("title")}</h1>
                <form className={styles.searchPanel} onSubmit={handleSubmit(onSubmit)}>
                    <div className={styles.searchField}>
                        <InputText name="lastName" label={t("search.lastName")} type="text" required={false} register={register} error={errors.lastName}/>
                    </div>
                    <div className={styles.searchField}>
                        <InputText name="firstName" label={t("search.firstName")} type="text" required={false} register={register} error={errors.firstName}/>
                    </div>
                    <div className={styles.searchField}>
                        <InputText name="phoneNumber" label={t("search.phoneNumber")} type="text" required={false} register={register} error={errors.phoneNumber}/>
                    </div>
                    <div className={styles.searchField}>
                        <InputText name="birthDate" label={t("search.birthDate")} type="text" required={false} register={register} error={errors.birthDate}/>
                    </div>
                    <div className={styles.searchButton}>
                        <Button text={t("search.search")} onClick={handleSubmit(onSubmit)}/>
                    </div>
                </form>
            </div>
            <div className={styles.searchResults}>
                {patients && <>
                    <h2>{t("results.title")}</h2>
                    {patients.length !== 0 && <div className={styles.searchResultsTable}>
                        <div className={styles.searchResultsRow}>
                            <div className={styles.searchResultsHeader}>{t("search.lastName")}</div>
                            <div className={styles.searchResultsHeader}>{t("search.firstName")}</div>
                            <div className={styles.searchResultsHeader}>{t("search.phoneNumber")}</div>
                            <div className={styles.searchResultsHeader}>{t("search.birthDate")}</div>
                        </div>
                        {patients?.map(patient => <div className={styles.searchResultsRow + " " + styles.searchResultsRowPatient} key={patient.id}>
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
            </div>
        </>
    </ContentContainer>
}

export default Patients;