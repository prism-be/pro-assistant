import {jsonToGraphQLQuery} from "json-to-graphql-query";
import {queryItems} from "../ajaxHelper";
import {formatISO} from "date-fns";

export interface Meeting {
    id?: string;
    patientId: string | null;
    title: string;
    firstName: string;
    lastName: string;
    startDate: string;
    duration: number;
    state: number;
    price: number;
    payment: number;
    paymentDate: string | null;
    type: string;
}

export const upsertMeeting = async (meeting: Meeting): Promise<boolean> => {
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

    const result = await queryItems<any>(graph);

    return result !== null;
}

export const getMeetingById = async (meetingId: string): Promise<Meeting> => {
    const query = {
        query: {
            meetingById: {
                __args: {
                    id: meetingId
                },
                id: true,
                patientId:true,
                title:true,
                firstName: true,
                lastName: true,
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

    return (await queryItems<any>(graph)).data.meetingById;
}

export const getMeetings = async (startDate: Date, endDate: Date): Promise<Meeting[]> => {
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
                firstName: true,
                lastName: true,
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

    return (await queryItems<any>(graph)).data.meetings;
}