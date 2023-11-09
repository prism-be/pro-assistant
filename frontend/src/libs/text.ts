export function isNullOrEmpty(value: string | null | undefined): boolean {
    return value == null || value === "";
}

export function onlyUnique(value: any, index: any, array: string | any[]) {
    return array.indexOf(value) === index;
}