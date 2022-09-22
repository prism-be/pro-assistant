import styles from "../styles/pages/login.module.scss";

import {NextPage} from "next";
import useTranslation from 'next-translate/useTranslation'
import {useAuth0} from "@auth0/auth0-react";

import Popin from "../components/Popin";
import Button from "../components/forms/Button";

const Login: NextPage = () => {

    const {t} = useTranslation("login");
    const {loginWithRedirect} = useAuth0();


    return <Popin>
        <>
            <h1 className={styles.title}>{t("title")}</h1>
            <Button text={t("form.go")} onClick={() => loginWithRedirect()}/>
        </>
    </Popin>
}

export default Login;