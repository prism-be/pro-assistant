import styles from "../styles/pages/login.module.scss";

import {useState} from "react";

import {NextPage} from "next";
import useTranslation from 'next-translate/useTranslation'

import {useForm} from "react-hook-form";
import * as yup from "yup";
import {yupResolver} from "@hookform/resolvers/yup";

import Popin from "../components/Popin";
import {postData} from "../lib/ajaxHelper";
import useUser from "../lib/useUser";
import {useRouter} from "next/router";
import InputText from "../components/forms/InputText";
import Button from "../components/forms/Button";

const Login: NextPage = () => {

    const {t, lang} = useTranslation("login");
    const {mutateUser} = useUser();
    const router = useRouter();
    const [errorMessage, setErrorMessage] = useState('');

    const schema = yup.object({
        login: yup.string().required(t("form.validation.required", {ns: 'common'})),
        password: yup.string().required(t("form.validation.required", {ns: 'common'})),
    }).required();

    const {register, handleSubmit, formState: {errors}} = useForm({resolver: yupResolver(schema)});

    const onSubmit = async (data: any) => {
        setErrorMessage("");
        const result = await postData('/authentication/login', data);

        if (result.status === 200) {
            localStorage.setItem('accessToken', result.data.accessToken);
            localStorage.setItem('refreshToken', result.data.refreshToken);

            await mutateUser();
            await router.push('/');

            return;
        }

        setErrorMessage(t("login:error.invalidCredentials"));
    }

    return <Popin>
        <>
            <h1 className={styles.title}>{t("title")}</h1>
            <form onSubmit={handleSubmit(onSubmit)}>
                <InputText name="login" label={t("form.login")} type="text"
                           required={true} register={register} error={errors.login}/>
                <InputText name="password" label={t("form.password")} type="password"
                           required={true} register={register} error={errors.password}/>
                <div className={styles.button}>
                    <Button text={t("form.go")}/>
                </div>
            </form>
        </>
    </Popin>
}

export default Login;