import {msalInstance} from "../../services/msal.ts";
import {currentUser$} from "../store.ts";
import {Popin} from "./Pops.tsx";
import Button from "../forms/Button.tsx";
import {useTranslation} from "react-i18next";

export default function LoginPanel() {

    const authenticated = currentUser$.isAuthenticated.use();
    const checked = currentUser$.checked.use();
    
    const {t} = useTranslation();

    return <>
        { checked && !authenticated &&
            <Popin>
                    <h1 className={"font-medium"}>{t("login.title")}</h1>
                    <div className={"m-auto max-w-md pt-4"}>
                        <Button text={t("login.form.go")} onClick={() => msalInstance.loginRedirect()}/>
                    </div>
            </Popin>
        }
    </>
}