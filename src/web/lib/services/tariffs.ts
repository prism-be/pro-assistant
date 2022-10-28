import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";
import {EnumType, jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";

export interface Tariff {
    id: string;
    name: string;
    price: number;
    defaultDuration: number;
}

export const upsertTariff = async (tariff: Tariff, instance: IPublicClientApplication, account: AccountInfo): Promise<boolean> => {
    const query = {
        mutation : {
            upsertTariff: {
                __args: {
                    tariff
                },
                id: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    const result = await queryItems<any>(instance, account, graph);

    return result !== null;
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
                defaultDuration: true
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