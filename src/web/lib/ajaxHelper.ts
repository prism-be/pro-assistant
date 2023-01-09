import {InteractionRequiredAuthError} from "@azure/msal-browser";
import {msalInstance} from "./msal";


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

export async function generateDocument<TResult>(documentId: string, appointmentId: string): Promise<void> {
    const bearer = await getAuthorization();

    const body = {
        documentId,
        appointmentId: appointmentId
    };
    await fetch("/api/document/generate", {
        method: "POST",
        body: JSON.stringify(body),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });
}

export async function downloadDocument<TResult>(documentId: string): Promise<void> {
    const bearer = await getAuthorization();

    const body = {
        documentId
    };

    const response = await fetch("/api/downloads/start", {
        method: "POST",
        body: JSON.stringify(body),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        const data = await response.json();
        window.open("/api/downloads/" + data.key, "_blank");
    }
}

function globalTrim(obj: any) {
    Object.keys(obj).forEach(k => obj[k] = typeof obj[k] == 'string' ? obj[k].trim() : obj[k]);
}

export async function postData<TResult>(route: string, body: any): Promise<TResult | null> {

    globalTrim(body);

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

export async function deleteDataWithBody<TResult>(route: string, body: any): Promise<void> {

    const bearer = await getAuthorization();

    await fetch("/api" + route, {
        body: JSON.stringify(body),
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
        scopes: ["https://" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID + "/records.manage"],
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