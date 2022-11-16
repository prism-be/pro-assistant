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

/*
export async function postFile(route: string, file: File): Promise<any> {
    
    const data = new FormData();
    data.append('file', file, file.name);
    
    const response = await fetch(config.apiRoot + route, {
        body: data,
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Authorization': await getAuthorization()
        },
    });

    if (response.status === 401)
    {
        if (await performRefreshToken())
        {
            return postFile(route, file);
        }
    }

    if (response.status === 200)
    {
        return {
            status: response.status,
            data: undefined
        }
    }

    return {
        status: response.status,
        data: undefined
    }
}*/

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