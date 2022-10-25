import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";
import {EnumType, jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";

export interface Tariff {
    id: string;
    name: string;
    price: number;
}

export const getTariffs = async (instance: IPublicClientApplication, account: AccountInfo): Promise<Tariff[]> => {

    const query = {
        query: {
            tariffs: {
                __args: {
                    order: {
                        name: new EnumType('ASC')
                    },
                },
                id: true,
                name: true,
                price: true,
            }
        }
    }

    const graph = jsonToGraphQLQuery(query, {pretty: true});

    const result = await queryItems<any>(instance, account, graph);

    if (result.data) {
        return result.data.tariffs;
    }

    return [];
}