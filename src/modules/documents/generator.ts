import {DocumentRequest} from "@/modules/documents/types";
import {Db, GridFSBucket, ObjectId} from "mongodb";
import {Appointment, BinaryDocument, Contact, DocumentConfiguration, Setting} from "@/libs/models";
import { jsPDF } from "jspdf";
import {format, formatISO, parseISO} from "date-fns";
import {formatAmount, replaceSpecialChars} from "@/libs/formats";
import {getLocale} from "@/libs/localization";
import Mustache from "mustache";

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

async function getSetting(db: Db, id: string) {
    // @ts-ignore
    const setting = await db.collection("settings").findOne<Setting>({_id: id});
    if (!setting) {
        throw new Error("Setting not found");
    }
    return setting.value as string;
}

function getFormattedPayment(payment: number) {
    switch (payment) {
        case 0:
            return "Non payé";
        case 1:
            return "Payé en espèces";
        case 2:
            return "Payé par virement";
        case 3:
            return "Payé par carte bancaire";

    }
    
    throw new Error("Invalid payment");
}

async function replaceContent(db: Db, title: string, content: string, appointment: Appointment, contact: Contact) {
    const data = {
        name: await getSetting(db, "document-header-your-name"),
        contactName: `${contact.title} ${contact.lastName} ${contact.firstName}`,
        price: formatAmount(appointment.price),
        appointmentType: appointment.type,
        appointmentDate: format(parseISO(appointment.startDate), "dd/MM/yyyy", {locale: getLocale()}),
        appointmentHour: format(parseISO(appointment.startDate), "HH:mm", {locale: getLocale()}),
        paymentDate: format(parseISO(appointment.paymentDate ?? appointment.startDate), "dd/MM/yyyy", {locale: getLocale()}),
        paymentMode: getFormattedPayment(appointment.payment),
    }
    
    const formattedTitle = Mustache.render(title, data);
    const formattedContent = Mustache.render(content, data);
    
    return {formattedTitle, formattedContent};
}

export async function generateDocument<TResult>(db: Db, request: DocumentRequest): Promise<void> {
    const {title, body} = await getDocumentConfiguration(db, request.documentId);
    const appointment = await getAppointment(db, request.appointmentId);
    const contact = await getContact(db, appointment.contactId);
    const {formattedTitle, formattedContent} = await replaceContent(db, title ?? '', body ?? '', appointment, contact);
    const pdfBytes = generatePdf(formattedTitle, formattedContent, appointment, contact);
    await saveDocument(db, appointment, formattedTitle, pdfBytes);
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