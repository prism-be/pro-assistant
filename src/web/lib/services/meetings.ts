import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";
import {jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";
import {Tariff} from "./tariffs";

export interface Meeting {
    id?: string;
    patientId?: string | null;
    title?: string;
    startDate?: string;
    duration?: number;
    state?: number;
    price?: number;
    payment?: number;
    paymentDate?: string;
    type?: string;
}

export const upsertMeeting = async (meeting: Meeting, instance: IPublicClientApplication, account: AccountInfo): Promise<boolean> => {
    const query = {
        mutation : {
            upsertMeeting: {
                __args: {
                    meeting
                },
                id: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    const result = await queryItems<any>(instance, account, graph);

    return result !== null;
}