import getConfig from 'next/config'

interface ObjectResult
{
    status: number;
    data: any | undefined;
}

const { publicRuntimeConfig: config } = getConfig()

export async function getData(route: string): Promise<ObjectResult> {
    const response = await fetch(config.apiRoot + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': getAuthorization()
        },
    });
    
    if (response.status === 401)
    {
        if (await performRefreshToken())
        {
            return getData(route);
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
            'Authorization': getAuthorization()
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
            'Authorization': getAuthorization()
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
}

const getAuthorization = (): string => {
    const accessToken = localStorage.getItem('accessToken');

    if (accessToken) {
        return 'Bearer ' + accessToken;
    }

    return '';
}