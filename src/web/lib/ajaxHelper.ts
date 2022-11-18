import {InteractionRequiredAuthError} from "@azure/msal-browser";
import getConfig from 'next/config'
import {msalInstance} from "./msal";
const { publicRuntimeConfig: config } = getConfig()

export async function getData<TResult>(route: string): Promise<TResult | null> {
    const bearer = await getAuthorization();

    const response = await fetch("/api" + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        return await response.json()
    }

    return null;
}

export async function displayFile<TResult>(route: string, id: string): Promise<void> {
    const bearer = await getAuthorization();

    const response = await fetch(route + "/" + id, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        const data = await response.json();
        window.open(route + "/" + data.key, "_blank");
    }
}

export async function postData<TResult>(route: string, body: any): Promise<TResult | null> {

    const bearer = await getAuthorization();

    const response = await fetch("/api" + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        return await response.json();
    }

    return null;
}

export async function deleteData<TResult>(route: string): Promise<void> {

    const bearer = await getAuthorization();

    await fetch("/api" + route, {
        method: "DELETE",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });
}

export const getAuthorization = async (): Promise<string> => {
    const accessTokenRequest = {
        scopes: ["https://" + config.tenantName + ".onmicrosoft.com/" + config.clientId + "/records.manage"],
        account: msalInstance.getAllAccounts()[0],
    };

    let accessToken = '';

    try {
        const accessTokenResponse = await msalInstance.acquireTokenSilent(accessTokenRequest);

        if (accessTokenResponse?.accessToken) {
            accessToken = accessTokenResponse?.accessToken;
        }
    } catch (error) {
        console.log(error);

        if (error instanceof InteractionRequiredAuthError) {
            try {
                const accessTokenResponse = await msalInstance.acquireTokenPopup(accessTokenRequest);

                if (accessTokenResponse?.accessToken) {
                    accessToken = accessTokenResponse?.accessToken;
                }
            } catch (error) {
                console.log(error);
            }
        }
    }

    if (accessToken) {
        return 'Bearer ' + accessToken;
    }

    return '';
}