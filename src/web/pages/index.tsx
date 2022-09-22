import type {NextPage} from 'next'
import Loader from "../components/design/Loader";
import useUser from "../lib/useUser";
import styles from "../styles/pages/login.module.scss";
import Button from "../components/forms/Button";
import Popin from "../components/Popin";
import {useAuth0} from "@auth0/auth0-react";
import useTranslation from "next-translate/useTranslation";
import {
    AuthenticatedTemplate,
    UnauthenticatedTemplate,
    useMsal,
} from '@azure/msal-react';
import ProtectedComponent from "../components/ProtectedComponent";

const Home: NextPage = () => {

    const { instance, accounts } = useMsal();
    
    const {t} = useTranslation("login");

    console.log(accounts);
    
    return (
        <div>
            <AuthenticatedTemplate>
                <p>Welcome, {accounts[0]?.username}</p>
                <ProtectedComponent />
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
                <Popin>
                    <>
                        <h1 className={styles.title}>{t("title")}</h1>
                        <div className={styles.button}>
                            <Button text={t("form.go")} onClick={() => instance.loginRedirect()}/>
                        </div>
                    </>
                </Popin>
            </UnauthenticatedTemplate>
        </div>
    )
}

export default Home
