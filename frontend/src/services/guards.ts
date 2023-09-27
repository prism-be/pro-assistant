export function notNullOrEmpty(value: string | null | undefined, parameterName: string): string {
    if (!value) {
        throw new Error(`Parameter ${parameterName} cannot be null or empty`);
    }
    
    return value;
}