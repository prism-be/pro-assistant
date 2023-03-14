import {DocumentRequest} from "@/modules/documents/types";
import {Db, GridFSBucket, ObjectId} from "mongodb";
import {Appointment, BinaryDocument, Contact, Setting} from "@/libs/models";
import {jsPDF} from "jspdf";
import {format, formatISO, parseISO} from "date-fns";
import {formatAmount, replaceSpecialChars} from "@/libs/formats";
import {getLocale} from "@/libs/localization";
import Mustache from "mustache";
import sharp from "sharp";

const documentMargin = 1;

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

async function resizeImage(image: string, w: number, h: number) {
    return await sharp(Buffer.from(Buffer.from(image.split(';base64,').pop() as string, 'base64')))
        .resize(w, h, {fit: "contain", background: "white"})
        .png()
        .toBuffer();
}

function hexToRgb(color: string) {
    const match = color.replace(/#/, '').match(/.{1,2}/g);
    if (!match) {
        return {r: 0, g: 0, b: 0};
    }
    const r = parseInt(match[0], 16);
    const g = parseInt(match[1], 16);
    const b = parseInt(match[2], 16);
    return {r, g, b};
}

async function writeHeader(db: Db, doc: jsPDF) {
    const accentuateColor = hexToRgb(await getSetting(db, "document-header-accentuate-color"));
    const headerName = await getSetting(db, "document-header-name");
    const headerAddress = await getSetting(db, "document-header-address");
    const logo = await getSetting(db, "document-header-logo");
    const resizedLogo = await resizeImage(logo, 250, 250);

    doc.setFontSize(10);
    doc.setFont("helvetica", "normal");

    doc.addImage(resizedLogo, "PNG", 1, 1, 2, 2);
    doc.text(headerName + "\n" + headerAddress, 3.25, 1.5);
    doc.setDrawColor(accentuateColor.r, accentuateColor.g, accentuateColor.b);
    doc.setLineWidth(0.01);
    doc.line(1, 3.25, doc.internal.pageSize.width - documentMargin, 3.25);
}

async function writeAddress(db: Db, doc: jsPDF, contact: Contact) {
    doc.setFont("helvetica", "normal");

    doc.setFontSize(10);
    const date = format(new Date(), "iiii dd MMMM yyyy", {locale: getLocale()});
    const dateWidth = doc.getTextWidth(date);
    doc.text(date, doc.internal.pageSize.width - documentMargin - dateWidth, 4);

    doc.setFontSize(12);
    let contactAddress = `${contact.title ?? ''} ${contact.lastName ?? ''} ${contact.firstName ?? ''}`.trim() + "\n";
    contactAddress += `${contact.street ?? ''} ${contact.number ?? ''}`.trim() + "\n";
    contactAddress += `${contact.zipCode ?? ''} ${contact.city ?? ''}`.trim() + "\n";
    contactAddress += `${contact.country ?? ''}`.trim() + "\n";
    doc.text(contactAddress, 11, 5.5);
}

async function writeContent(doc: jsPDF, title: string, content: string) {
    doc.setFontSize(12);

    doc.setFont("helvetica", "bold");
    doc.text(title, documentMargin, 10);

    doc.setFont("helvetica", "normal");
    doc.text(doc.splitTextToSize(content, doc.internal.pageSize.width - documentMargin * 2), documentMargin, 11);
}

async function writeFooter(db: Db, doc: jsPDF) {
    const name = await getSetting(db, "document-header-your-name");
    const city = await getSetting(db, "document-header-your-city");
    const signature = await getSetting(db, "document-header-signature");
    const resizedSignature = await resizeImage(signature, 400, 200);

    doc.addImage(resizedSignature, "PNG", doc.internal.pageSize.width - 8, doc.internal.pageSize.height - 6, 6.5, 2);

    doc.setFontSize(10);
    doc.setFont("helvetica", "normal");

    const signatureContent = `${name}\n${city}, ${format(new Date(), "iiii dd MMMM yyyy", {locale: getLocale()})}`;
    const signatureWidth = doc.getTextWidth(signatureContent);
    doc.text(signatureContent, doc.internal.pageSize.width - documentMargin - signatureWidth, doc.internal.pageSize.height - 3);
}

async function generatePdf(db: Db, title: string, content: string, appointment: Appointment, contact: Contact): Promise<ArrayBuffer> {

    const doc = new jsPDF({unit: "cm", format: "a4"});

    await writeHeader(db, doc);
    await writeAddress(db, doc, contact);
    await writeContent(doc, title, content);
    await writeFooter(db, doc);

    return doc.output("arraybuffer");
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
    if (!existing) {
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
    const formattedContent = Mustache.render(content.replaceAll("{{", "{{{").replaceAll("}}", "}}}"), data);

    return {formattedTitle, formattedContent};
}

export async function generateDocument<TResult>(db: Db, request: DocumentRequest): Promise<void> {
    const {title, body} = await getDocumentConfiguration(db, request.documentId);
    const appointment = await getAppointment(db, request.appointmentId);
    const contact = await getContact(db, appointment.contactId);
    const {formattedTitle, formattedContent} = await replaceContent(db, title ?? '', body ?? '', appointment, contact);
    const pdfBytes = await generatePdf(db, formattedTitle, formattedContent, appointment, contact);
    await saveDocument(db, appointment, formattedTitle, pdfBytes);
}

export async function deleteDocument<TResult>(db: Db, request: DocumentRequest): Promise<void> {
    const bucket = new GridFSBucket(db);
    await bucket.delete(new ObjectId(request.documentId));

    const appointment = await getAppointment(db, request.appointmentId);
    const existing = await db.collection("appointments").findOne<Appointment>({_id: new ObjectId(appointment._id)});
    if (!existing) {
        throw new Error("Appointment not found");
    }

    existing.documents = existing.documents.filter(d => d.id !== request.documentId);
    await db.collection("appointments").updateOne({_id: new ObjectId(appointment._id)}, {$set: existing});
}