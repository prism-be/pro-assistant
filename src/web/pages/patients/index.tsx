import styles from '../../styles/pages/patients.module.scss';

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import Button from "../../components/forms/Button";

const Patients: NextPage = () => {

    const {t} = useTranslation('patients');

    const schema = yup.object({}).required();

    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});

    const onSubmit = async (data: any) => {
        console.log(data);
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

            </div>
        </>
    </ContentContainer>
}

export default Patients;