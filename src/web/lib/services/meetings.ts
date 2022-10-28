import {AccountInfo, IPublicClientApplication} from "@azure/msal-browser";
import {jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";
import {formatISO} from "date-fns";

export interface Meeting {
    id?: string;
    patientId?: string | null;
    title?: string;
    startDate: string;
    duration: number;
    state?: number;
    price?: number;
    payment?: number;
    paymentDate?: string | null;
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

export const getMeetings = async (startDate: Date, endDate: Date, instance: IPublicClientApplication, account: AccountInfo): Promise<Meeting[]> => {
    const query = {
        query: {
            meetings: {
                __args: {
                    startDate: formatISO(startDate),
                    endDate: formatISO(endDate)
                },
                id: true,
                patientId:true,
                title:true,
                startDate:true,
                duration: true,
                state: true,
                price: true,
                payment:true,
                paymentDate: true,
                type: true,
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    return (await queryItems<any>(instance, account, graph)).data.meetings;
}