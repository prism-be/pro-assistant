import {AccountInfo, InteractionRequiredAuthError, IPublicClientApplication} from "@azure/msal-browser";
import getConfig from 'next/config'
import {msalInstance} from "./msal";
const { publicRuntimeConfig: config } = getConfig()

interface ObjectResult<TData> {
    status: number;
    data: TData | undefined;
}

export async function getData<TResult>(route: string): Promise<ObjectResult<TResult>> {
    const bearer = await getAuthorization();

    const response = await fetch(route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        return {
            status: response.status,
            data: await response.json()
        }
    }

    return {
        status: response.status,
        data: undefined
    }
}

export async function postData<TResult>(route: string, body: any): Promise<ObjectResult<TResult>> {

    const bearer = await getAuthorization();

    const response = await fetch(route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        return {
            status: response.status,
            data: await response.json()
        }
    }

    return {
        status: response.status,
        data: undefined
    }
}

export async function queryItems<TResult>(query: string): Promise<TResult> {
    const bearer = await getAuthorization();

    const body = {

        query: query
    };

    const response = await fetch("/api/graphql", {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    if (response.status === 200) {
        return response.json();
    }

    throw response;
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

export const buildWhere= (query: any) => {
    let where: any = {};

    Object.getOwnPropertyNames(query).forEach(p => {
        if (query[p] && query[p] != "" && query[p] != 0)
        {
            where[p] = {
                contains: query[p]
            }
        }
    })

    return where;
}