import { Contact, Tariff } from "@/libs/models";
import logger from "@/libs/logging";
import { Db } from "mongodb";

async function updateTariffsColors(db: Db, id: string, tariff: Tariff) {
    logger.info("Updating tariffs colors for tariff " + id + " to " + tariff.backgroundColor + " and " + tariff.foreColor);
    await db.collection("appointments").updateMany(
        { typeId: id },
        {
            $set: {
                backgroundColor: tariff.backgroundColor,
                foreColor: tariff.foreColor,
            },
        }
    );
}

async function updateContactInformation(db: Db, id: string, value: Contact) {
    logger.info("Updating contact information for contact " + id + " to " + JSON.stringify(value));
    await db.collection("appointments").updateMany(
        { contactId: id },
        {
            $set: {
                firstName: value.firstName,
                lastName: value.lastName,
                birthDate: value.birthDate,
                phoneNumber: value.phoneNumber,
                title: value.lastName + " " + value.firstName,
            },
        }
    );
}

export async function processSideEffects(collection: string, id: string, value: any, db: Db) {
    switch (collection) {
        case "tariffs":
            await updateTariffsColors(db, id, value as Tariff);
            break;
        case "contacts":
            await updateContactInformation(db, id, value as Contact);
            break;
    }
}
