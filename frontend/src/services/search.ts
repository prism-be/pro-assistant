import type {Contact, Filter} from "./models.ts";
import {postData} from "./http.ts";

export async function searchContacts(filter: any): Promise<Contact[]> {
    let filters : Filter[] = [];

    if (filter.lastName) {
        filters.push({ field : "LastName", value: `^${filter.lastName}`, operator: "regex" });
    }

    if (filter.firstName) {
        filters.push({ field : "FirstName", value: `^${filter.firstName}`, operator: "regex" });
    }

    if (filter.birthDate) {
        filters.push({ field : "BirthDate", value: `${filter.birthDate}`, operator: "regex" });
    }

    if (filter.phoneNumber) {
        filters.push({ field : "PhoneNumber", value: `${filter.phoneNumber}`, operator: "regex" });
    }

    return await postData<Contact[]>("/data/contacts/search", filters);
}
