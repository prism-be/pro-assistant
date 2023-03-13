export async function getData<TResult>(route: string): Promise<TResult | null> {

    const response = await fetch("/api" + route, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    });

    if (response.status === 200) {
        return await response.json()
    }

    return null;
}

export async function generateDocument<TResult>(documentId: string, appointmentId: string): Promise<void> {

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
        },
    });
}

export async function downloadDocument<TResult>(documentId: string): Promise<void> {

    const body = {
        documentId
    };

    const response = await fetch("/api/downloads/start", {
        method: "POST",
        body: JSON.stringify(body),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
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

    const response = await fetch("/api" + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    });

    if (response.status === 200) {
        return await response.json();
    }

    return null;
}

export async function putData(route: string, body: any): Promise<void> {

    globalTrim(body);


    await fetch("/api" + route, {
        body: JSON.stringify(body),
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    });
}

export async function deleteData<TResult>(route: string): Promise<void> {


    await fetch("/api" + route, {
        method: "DELETE",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    });
}

export async function deleteDataWithBody<TResult>(route: string, body: any): Promise<void> {

    await fetch("/api" + route, {
        body: JSON.stringify(body),
        method: "DELETE",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    });
}