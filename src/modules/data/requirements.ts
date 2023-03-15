import { Appointment, Contact, Tariff } from "@/libs/models";
import { Db } from "mongodb";
import { isNullOrEmpty } from "@/libs/text";
import logger from "@/libs/logging";

async function ensureContact(db: Db, id: string, appointment: Appointment) {
    if (isNullOrEmpty(appointment.contactId)) {
        logger.info("Creating contact for appointment " + id + " with " + JSON.stringify(appointment));

        const contact: Contact = {
            firstName: appointment.firstName,
            lastName: appointment.lastName,
            birthDate: appointment.birthDate,
            phoneNumber: appointment.phoneNumber,
        };

        const result = await db.collection("contacts").insertOne(contact);
        appointment.contactId = result.insertedId.toHexString();
    }
}

export async function processRequirements(collection: string, id: string, value: any, db: Db) {
    switch (collection) {
        case "appointments":
            await ensureContact(db, id, value as Appointment);
            break;
    }
}
