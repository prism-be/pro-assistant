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

export async function generateDocument(documentId: string, appointmentId: string): Promise<void> {
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

export async function downloadDocument(documentId: string): Promise<void> {
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

export async function postData<TResult>(route: string, body: any): Promise<TResult> {

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

    return await response.json();
}

export async function putData(route: string, body: any): Promise<void> {

    globalTrim(body);

    const bearer = await getAuthorization();

    await fetch("/api" + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });
}

export async function deleteData(route: string): Promise<void> {

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

export async function deleteDataWithBody(route: string, body: any): Promise<void> {

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

    const activeAccount = msalInstance.getActiveAccount();
    const accounts = msalInstance.getAllAccounts();

    if (!activeAccount && accounts.length === 0) {
        return '';
    }
    
    const accessTokenRequest = {
        scopes: ["https://" + import.meta.env.VITE_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + import.meta.env.VITE_PUBLIC_AZURE_AD_CLIENT_ID + "/records.manage"],
        account: msalInstance.getAllAccounts()[0],
    };

    const authResult = await msalInstance.acquireTokenSilent(accessTokenRequest);

    if (authResult.accessToken) {
        return 'Bearer ' + authResult.accessToken;
    }

    return '';
}