import styles from '../../styles/pages/patients.module.scss';

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

const Patients: NextPage = () => {

    const {t} = useTranslation('patients');

    const schema = yup.object({
    }).required();

    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});

    const onSubmit = async (data: any) => {
        console.log(data);
    }

    return <ContentContainer>
        <>
            <h1>{t("title")}</h1>
            <form className={styles.searchPanel} onSubmit={handleSubmit(onSubmit)}>
                <InputText name="firstName" label={t("search.firstName")} type="text" required={false} register={register} error={errors.firstName}/>
            </form>
        </>
    </ContentContainer>
}

export default Patients;