import {Contact} from "@/libs/models";
import {postData} from "@/libs/http";

export async function searchContacts(filter: any): Promise<Contact[]> {
    return await postData<Contact[]>("/data/contacts/search", filter);
}
