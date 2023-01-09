﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.18.2.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming



export interface Appointment {
    startDate: string;
    paymentDate?: string | null;
    price: number;
    duration: number;
    payment: number;
    state: number;
    firstName: string;
    id: string;
    lastName: string;
    title: string;
    patientId?: string | null;
    type?: string | null;
    typeId?: string | null;
    foreColor?: string | null;
    backgroundColor?: string | null;
    documents: BinaryDocument[];
}

export interface BinaryDocument {
    date: string;
    fileName: string;
    id: string;
    title: string;
}

export interface DeleteDocument {
    id: string;
    appointmentId: string;
}

export interface DocumentConfiguration {
    body?: string | null;
    name?: string | null;
    title?: string | null;
    id: string;
}

export interface DownloadDocument {
    documentId: string;
}

export interface DownloadKey {
    key: string;
}

export interface GenerateDocument {
    documentId: string;
    appointmentId: string;
}

export interface Patient {
    id: string;
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
    zipCode?: string | null;
    title?: string | null;
}

export interface SearchAppointments {
    startDate: string;
    endDate: string;
    patientId?: string | null;
}

export interface SearchPatients {
    lastName: string;
    firstName: string;
    phoneNumber: string;
    birthDate: string;
}

export interface Setting {
    id: string;
    value?: string | null;
}

export interface Tariff {
    price: number;
    defaultDuration: number;
    id: string;
    name: string;
    foreColor?: string | null;
    backgroundColor?: string | null;
}

export interface UpsertResult {
    id: string;
    organization: string;
}

export interface UserInformation {
    name?: string | null;
    authenticated: boolean;
}