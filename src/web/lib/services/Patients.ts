import {postData, queryItems} from "../ajaxHelper";
import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";

export interface SearchParameter {
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export interface PatientSummary {
    id: string;
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export const searchPatients = async (query: SearchParameter, instance: IPublicClientApplication, account: AccountInfo): Promise<PatientSummary[]> => {

    const graph = `query patients {
                    patients (order: {  firstName: ASC, lastName: ASC, id: ASC}) {
                        nodes {
                            id,
                            firstName,
                            lastName,
                            phoneNumber,
                            birthDate
                        }
                    }
                }`

    const result = await queryItems<any>(instance, account, graph);

    if (result.data) {
        return result.data.patients.nodes;
    }

    return [];
}