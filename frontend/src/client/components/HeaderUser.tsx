import {getData} from "../../services/http.ts";
import {msalInstance} from "../../services/msal.ts";
import {currentUser$} from "../store.ts";
import {useMountOnce, useObserve} from "@legendapp/state/react";
import {useTranslation} from "react-i18next";

export default function HeaderUser() {

    const {t} = useTranslation('translation');

    const name = currentUser$.name.use();
    const authenticated = currentUser$.isAuthenticated.use();
    const checked = currentUser$.checked.use();

    useMountOnce(() => {
        checkUser();
    });

    useObserve(currentUser$.redirected, (redirected) => {
        if (redirected.value) {
            checkUser();
        }
    });

    function checkUser() {
        getData("/authentication/user").then((data: any) => {
            currentUser$.isAuthenticated.set(true);
            currentUser$.name.set(data.name);
        })
            .finally(() => {
                currentUser$.checked.set(true);
            });
    }

    return <>
        {checked && authenticated && <>
            {t("header.hello", {name})}<br />
            <a href="#" onClick={async () => await msalInstance.logoutRedirect()}>{t("header.logout")}</a>
        </>}
    </>
}