import {format, parseISO} from "date-fns";
import {getLocale} from "@/libs/localization";

export function formatAmount(value: number | string) {
    if (value as string) {
        value = parseFloat(value as string);
    }

    return (value as number).toFixed(2);
}

export function formatIsoDate(value: string) {
    const d = parseISO(value);
    return format(d, 'dd/MM/yyyy');
}

export function formatIsoMonth(value: string) {
    const d = parseISO(value);
    return format(d, 'MMMM yyyy', {locale: getLocale()});
}