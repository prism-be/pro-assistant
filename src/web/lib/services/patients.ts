import {queryItems} from "../ajaxHelper";
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

export interface Patient {
    id: string;
    lastName: string;
    firstName: string;
    phoneNumber: string;
    email: string;
    birthDate: string;
    street:string;
    number: string;
    zipCode:string;
    city:string;
    country:string;
}

export const createPatient = async (patient: Patient): Promise<boolean> => {
    
    patient.id = "";
    
    const query = {
        mutation : {
            createPatient: {
                __args: {
                    patient
                },
                id: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    const result = await queryItems<any>(graph);

    return result.data.createPatient.id;
}

export const savePatient = async (patient: Patient): Promise<boolean> => {
    const query = {
        mutation : {
            updatePatient: {
                __args: {
                    patient
                },
                id: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    const result = await queryItems<any>(graph);

    return result !== null;
}

export const getPatient = async (id: string): Promise<Patient | null> => {
    const query = {
        query : {
            patientById: {
                __args: {
                    id
                },
                id: true,
                lastName: true,
                firstName: true,
                phoneNumber: true,
                email: true,
                birthDate: true,
                street:true,
                number: true,
                zipCode:true,
                city:true,
                country:true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query);

    const result = await queryItems<any>(graph);

    if (result.data) {
        return result.data.patientById;
    }

    return null;
}

export const searchPatients = async (search: SearchParameter): Promise<PatientSummary[]> => {

    const query = {
        query: {
            searchPatients: {
                __args: {
                    order: {
                        lastName: new EnumType('ASC'),
                        firstName: new EnumType('ASC')
                    },
                    lastName: search.lastName,
                    firstName: search.firstName,
                    phoneNumber: search.phoneNumber,
                    birthDate: search.birthDate,
                },
                id: true,
                firstName: true,
                lastName: true,
                phoneNumber: true,
                birthDate: true
            }
        }
    }

    const graph = jsonToGraphQLQuery(query, {pretty: true});

    const result = await queryItems<any>(graph);

    if (result.data) {
        return result.data.searchPatients;
    }

    return [];
}