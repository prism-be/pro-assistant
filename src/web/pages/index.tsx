import type {NextPage} from 'next'
import styles from "../styles/pages/login.module.scss";
import Button from "../components/forms/Button";
import Popin from "../components/Popin";
import useTranslation from "next-translate/useTranslation";
import {
    AuthenticatedTemplate,
    UnauthenticatedTemplate,
    useMsal,
} from '@azure/msal-react';
import ProtectedComponent from "../components/ProtectedComponent";
import Header from "../components/design/Header";

const Home: NextPage = () => {

    const { instance, accounts } = useMsal();
    
    const {t} = useTranslation("login");

    return (
        <div>
            <AuthenticatedTemplate>
                <Header />
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
