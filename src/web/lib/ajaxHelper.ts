import {AccountInfo, InteractionRequiredAuthError, IPublicClientApplication} from "@azure/msal-browser";

interface ObjectResult {
    status: number;
    data: any | undefined;
}

export async function getData(route: string, instance: IPublicClientApplication, account: AccountInfo): Promise<ObjectResult> {
    
    console.log(route);
    const bearer = await getAuthorization(instance, account);
    
    console.log(bearer);
    
    const response = await fetch(route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': bearer
        },
    });

    console.log(response);
    
    if (response.status === 200)
    {
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

/*
export const performRefreshToken = async (): Promise<boolean> => {
    const refreshToken = localStorage.getItem('refreshToken');

    if (refreshToken)
    {
        const data = {
            refreshToken
        };
        
        const refreshResponse = await fetch(config.apiRoot + '/api/authentication/refresh', {
            body: JSON.stringify(data),
            method: "POST",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            }
        });

        if (refreshResponse.status === 200)
        {
            const refreshData = await refreshResponse.json();
            localStorage.setItem('accessToken', refreshData.accessToken);
            localStorage.setItem('refreshToken', refreshData.refreshToken);

            if (autoRefreshToken)
            {
                clearTimeout(autoRefreshToken);
            }
            autoRefreshToken = setTimeout(performRefreshToken, refreshData.expires / 2 * 1000);
            
            return true;
        }

        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        
        return false;
    }
    
    return false;
}

let autoRefreshToken: NodeJS.Timeout;

export async function postData(route: string, body: any): Promise<any> {
    const response = await fetch(config.apiRoot + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': await getAuthorization()
        },
    });

    if (response.status === 401)
    {
        if (await performRefreshToken())
        {
            return postData(route, body);
        }
    }

    if (response.status === 200)
    {
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

export const getAuthorization = async (instance: IPublicClientApplication, account: AccountInfo): Promise<string> => {
    const accessTokenRequest = {
        scopes: ["https://byprism.onmicrosoft.com/b210005a-b610-43e2-9dd5-824e50b9f692/records.manage"],
        account: account,
    };

    let accessToken = '';

    try {
        const accessTokenResponse = await instance.acquireTokenSilent(accessTokenRequest);
        
        if (accessTokenResponse?.accessToken) {
            accessToken = accessTokenResponse?.accessToken;
        }
    } catch (error) {
        console.log(error);

        if (error instanceof InteractionRequiredAuthError) {
            try {
                const accessTokenResponse = await instance.acquireTokenPopup(accessTokenRequest);

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