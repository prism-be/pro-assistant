import type {NextPage} from 'next'
import Loader from "../components/design/Loader";
import useUser from "../lib/useUser";
import styles from "../styles/pages/login.module.scss";
import Button from "../components/forms/Button";
import Popin from "../components/Popin";
import {useAuth0} from "@auth0/auth0-react";
import useTranslation from "next-translate/useTranslation";

const Home: NextPage = () => {

    const {user, isAuthenticated, loginWithRedirect} = useAuth0();
    const {t} = useTranslation("login");

    if (!user || !isAuthenticated) {
        return <Popin>
            <>
                <h1 className={styles.title}>{t("title")}</h1>
                <div className={styles.button}>
                    <Button text={t("form.go")} onClick={() => loginWithRedirect()}/>
                </div>
            </>
        </Popin>
    }

    return (
        <div>
        </div>
    )
}

export default Home
