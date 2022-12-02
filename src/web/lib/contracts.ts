export interface IUpsertResult
{
    id: string;
}

export interface ISearchPatient {
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export interface IPatientSummary {
    id: string;
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export interface IPatient {
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

export interface IMeeting {
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
    foreColor?: string;
    backgroundColor?: string;
}

export interface ITariff {
    id: string;
    name: string;
    price: number;
    defaultDuration: number;
    foreColor?: string;
    backgroundColor?: string;
}

export interface IDocumentSettings {
    name?: string;
    address?: string;
    logo?: string;
}

export interface IDocumentConfiguration {
    id?: string;
    name?: string;
    title?: string;
    body?: string;
}