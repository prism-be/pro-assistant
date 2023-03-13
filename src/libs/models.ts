import {ObjectId} from "mongodb";

export interface Appointment {
    startDate: string;
    paymentDate?: string | null;
    price: number;
    duration: number;
    payment: number;
    state: number;
    documents: BinaryDocument[];
    firstName: string;
    lastName: string;
    birthDate?: string | null;
    phoneNumber?: string | null;
    title: string;
    backgroundColor?: string | null;
    contactId?: string | null;
    foreColor?: string | null;
    type?: string | null;
    typeId?: string | null;
    _id: ObjectId
}

export interface BinaryDocument {
    date: string;
    fileName: string;
    _id: ObjectId
    title: string;
}

export interface Contact {
    birthDate?: string | null;
    city?: string | null;
    country?: string | null;
    email?: string | null;
    firstName?: string | null;
    lastName?: string | null;
    mobileNumber?: string | null;
    number?: string | null;
    phoneNumber?: string | null;
    street?: string | null;
    title?: string | null;
    zipCode?: string | null;
    _id: ObjectId
}

export interface DeleteDocument {
    _id: ObjectId
    appointment_id: ObjectId
}

export interface DocumentConfiguration {
    body?: string | null;
    name?: string | null;
    title?: string | null;
    _id: ObjectId
}

export interface DownloadDocument {
    document_id: ObjectId
}

export interface DownloadKey {
    key: string;
}

export interface GenerateDocument {
    document_id: ObjectId
    appointment_id: ObjectId
}

export interface SearchAppointments {
    startDate: string;
    endDate: string;
    contactId?: string | null;
}

export interface SearchContacts {
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export interface Setting {
    value: string;
    _id: ObjectId
}

export interface Tariff {
    price: number;
    defaultDuration: number;
    name: string;
    backgroundColor?: string | null;
    foreColor?: string | null;
    _id: ObjectId
}

export interface UpsertResult {
    _id: ObjectId
    organization: string;
}

export interface UserInformation {
    name?: string | null;
    organization?: string | null;
    authenticated: boolean;
}