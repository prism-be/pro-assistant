import {Contact} from "@/libs/models";
import {postData} from "@/libs/http";

export async function searchContacts(filter: any): Promise<Contact[]> {
    let query = {};

    if (filter.lastName) {
        query = {...query, lastName: {$regex: `^${filter.lastName}`, $options: 'i'}};
    }

    if (filter.firstName) {
        query = {...query, firstName: {$regex: `^${filter.firstName}`, $options: 'i'}};
    }

    if (filter.birthDate) {
        query = {...query, birthDate: {$regex: `${filter.birthDate}`, $options: 'i'}};
    }

    if (filter.phoneNumber) {
        query = {...query, phoneNumber: {$regex: `${filter.phoneNumber}`, $options: 'i'}};
    }

    return await postData<Contact[]>("/data/contacts/search", query);
}