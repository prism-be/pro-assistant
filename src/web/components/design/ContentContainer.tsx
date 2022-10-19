import styles from '../../styles/components/design/content-container.module.scss';

import Menu from "./Menu";
import {AuthenticatedTemplate, UnauthenticatedTemplate, useMsal} from "@azure/msal-react";
import Header from "./Header";
import Popin from "../Popin";
import Button from "../forms/Button";
import useTranslation from "next-translate/useTranslation";

interface Props {
    children: JSX.Element;
}

const ContentContainer = (props: Props) => {

    const {instance} = useMsal();
    const {t} = useTranslation("login");

    return <div>
        <AuthenticatedTemplate>
            <Header/>
            <div className={styles.container}>
                <Menu/>
                <div className={styles.content}>
                    {props.children}
                </div>
            </div>
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
}

export default ContentContainer;