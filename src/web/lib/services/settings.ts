import {jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";

export interface DocumentSettings {
    name?: string;
    address?: string;
    logo?: string;
}

export const getSettings = async <T>(key: 'documents-headers'): Promise<T> => {

    const query = {
        query: {
            settingById: {
                __args: {
                    id: key
                },
                id: true,
                value: true,
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);
    const value = (await queryItems<any>(graph)).data.settingById.value;

    return value ? JSON.parse(value) : {};
}

export const saveSettings = async <T>(key: 'documents-headers', value: T): Promise<boolean> => {

    const query = {
        mutation: {
            upsertSetting: {
                __args: {
                    setting: {
                        id: key,
                        value: JSON.stringify(value)
                    }
                },
                id: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);
    const result = await queryItems<any>(graph)
    return result != null;
}