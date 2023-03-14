import { remove } from "diacritics";

export function formatAmount(value: number | string) {
    if (value as string) {
        value = parseFloat(value as string);
    }

    return (value as number).toFixed(2);
}

export function replaceSpecialChars(value: string) {
    let processedString = value.replace(/\s+/g, "-");
    processedString = remove(processedString);
    processedString = processedString.replace(/-+/g, "-");
    processedString = processedString.toLowerCase();

    return processedString;
}
