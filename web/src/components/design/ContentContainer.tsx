import Menu from "./Menu";
import {AuthenticatedTemplate, UnauthenticatedTemplate, useMsal} from "@azure/msal-react";
import Header from "./Header";
import Button from "../forms/Button";
import {useTranslation} from "react-i18next";
import {Popin} from "../Pops";

interface Props {
    children: JSX.Element | JSX.Element[];
}

const ContentContainer = (props: Props) => {

    const {instance} = useMsal();
    const {t} = useTranslation("login");

    return <div>
        <AuthenticatedTemplate>
            <Header/>
            <div className={"p-0 m-0 border-box relative w-full"}>
                <Menu/>
                <div className={"md:ml-64"}>
                    {props.children}
                </div>
            </div>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <Popin>
                <>
                    <h1 className={"font-medium"}>{t("title")}</h1>
                    <div className={"m-auto max-w-md pt-4"}>
                        <Button text={t("form.go")} onClick={() => instance.loginRedirect()}/>
                    </div>
                </>
            </Popin>
        </UnauthenticatedTemplate>
    </div>
}

export default ContentContainer;