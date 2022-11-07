import {EnumType, jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";

export interface Tariff {
    id: string;
    name: string;
    price: number;
    defaultDuration: number;
}

export const upsertTariff = async (tariff: Tariff): Promise<boolean> => {
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

    const result = await queryItems<any>(graph);

    return result !== null;
}

export const getTariffs = async (): Promise<Tariff[]> => {

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

    const result = await queryItems<any>(graph);

    if (result.data) {
        return result.data.tariffs;
    }

    return [];
}