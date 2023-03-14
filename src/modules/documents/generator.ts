import {DocumentRequest} from "@/modules/documents/types";
import {Db, GridFSBucket, ObjectId} from "mongodb";
import {Appointment, BinaryDocument, Contact} from "@/libs/models";
import { jsPDF } from "jspdf";
import {format, formatISO} from "date-fns";
import {replaceSpecialChars} from "@/libs/formats";

async function getDocumentConfiguration(db: Db, documentId: string) {

    const configuration = await db.collection("documents-configuration").findOne({_id: new ObjectId(documentId)});

    if (!configuration) {
        throw new Error("Document configuration not found");
    }

    return configuration;
}

async function getAppointment(db: Db, appointmentId: string) {
    const appointment = await db.collection("appointments").findOne<Appointment>({_id: new ObjectId(appointmentId)});

    if (!appointment) {
        throw new Error("Appointment not found");
    }

    return appointment;
}

async function getContact(db: Db, contactId: any) {
    const contact = await db.collection("contacts").findOne<Contact>({_id: new ObjectId(contactId)});

    if (!contact) {
        throw new Error("Contact not found");
    }

    return contact;
}

function generatePdf(title: string, content: string, appointment: Appointment, contact: Contact): ArrayBuffer {
    const doc = new jsPDF();
    doc.line(10, 10, 200, 10);
    doc.text(title, 10, 20);
    
    return doc.output("arraybuffer")    ;
}

async function saveDocument(db: Db, appointment: Appointment, title: any, pdfBytes: ArrayBuffer) {
    let fileName = `${format(new Date(), "yyyy-MM-dd HH:mm")}- ${appointment.lastName} ${appointment.firstName} - ${title}.pdf`;
    fileName = replaceSpecialChars(fileName);
    
    const bucket = new GridFSBucket(db);
    const buffer = Buffer.from(pdfBytes);
    const uploadStream = bucket.openUploadStream(fileName);
    uploadStream.write(buffer);
    uploadStream.end();

    const document: BinaryDocument = {
        id: uploadStream.id.toHexString(),
        title,
        date: formatISO(new Date()),
        fileName: fileName
    };

    const existing = await db.collection("appointments").findOne<Appointment>({_id: new ObjectId(appointment._id)});
    if (!existing)
    {
        throw new Error("Appointment not found");
    }
    
    existing.documents.push(document);
    await db.collection("appointments").updateOne({_id: new ObjectId(appointment._id)}, {$set: existing});
}

export async function generateDocument<TResult>(db: Db, request: DocumentRequest): Promise<void> {
    const {title, content} = await getDocumentConfiguration(db, request.documentId);
    const appointment = await getAppointment(db, request.appointmentId);
    const contact = await getContact(db, appointment.contactId);
    const pdfBytes = generatePdf(title, content, appointment, contact);
    await saveDocument(db, appointment, title, pdfBytes);
}

export async function deleteDocument<TResult>(db: Db, request: DocumentRequest): Promise<void> {
    const bucket = new GridFSBucket(db);
    await bucket.delete(new ObjectId(request.documentId));
    
    const appointment = await getAppointment(db, request.appointmentId);
    const existing = await db.collection("appointments").findOne<Appointment>({_id: new ObjectId(appointment._id)});
    if (!existing)
    {
        throw new Error("Appointment not found");
    }
    
    existing.documents = existing.documents.filter(d => d.id !== request.documentId);
    await db.collection("appointments").updateOne({_id: new ObjectId(appointment._id)}, {$set: existing});
}