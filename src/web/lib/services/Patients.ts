import {queryItems} from "../ajaxHelper";
import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";
import {EnumType, jsonToGraphQLQuery} from "json-to-graphql-query";

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

const buildWhere= (query: any) => {
    let where: any = {};
    
    Object.getOwnPropertyNames(query).forEach(p => {
        if (query[p] && query[p] != "" && query[p] != 0)
        {
            where[p] = {
                contains: query[p]
            }
        }
    })
    
    return where;
}



export const searchPatients = async (search: SearchParameter, instance: IPublicClientApplication, account: AccountInfo): Promise<PatientSummary[]> => {

    const where = buildWhere(search);

    const query = {
        query : {
            patients: {
                __args: {
                    order: {
                        lastName: new EnumType('ASC'),
                        firstName: new EnumType('ASC')
                    },
                    where
                },
                nodes : {
                    id: true,
                    firstName: true,
                    lastName: true,
                    phoneNumber: true,
                    birthDate: true
                }
            }
        }
    }

    const graph = jsonToGraphQLQuery(query, { pretty: true });

    const result = await queryItems<any>(instance, account, graph);

    if (result.data) {
        return result.data.patients.nodes;
    }

    return [];
}