import {GenerateDocumentRequest} from "@/modules/documents/types";
import {Db, ObjectId} from "mongodb";

async function getDocumentConfiguration(db: Db, documentId: string) {

    const configuration = await db.collection("documents-configuration").findOne({_id: new ObjectId(documentId)});

    if (!configuration) {
        throw new Error("Document configuration not found");
    }

    return configuration;
}

async function getAppointment(db: Db, appointmentId: string) {
    const appointment = await db.collection("appointments").findOne({_id: new ObjectId(appointmentId)});

    if (!appointment) {
        throw new Error("Appointment not found");
    }

    return appointment;
}

async function getContact(db: Db, contactId: any) {
    const contact = await db.collection("contacts").findOne({_id: new ObjectId(contactId)});

    if (!contact) {
        throw new Error("Contact not found");
    }

    return contact;
}

export async function generateDocument<TResult>(db: Db, request: GenerateDocumentRequest): Promise<void> {
    const {title, content} = await getDocumentConfiguration(db, request.documentId);
    const appointment = await getAppointment(db, request.appointmentId);
    const contact = await getContact(db, appointment.contactId);
}