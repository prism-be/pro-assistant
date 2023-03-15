import { NextApiRequest, NextApiResponse } from "next";
import { Db, ObjectId } from "mongodb";
import logger from "@/libs/logging";
import { Contact, Tariff } from "@/libs/models";
import { getDatabaseAndCollection } from "@/libs/api";

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

async function processSideEffects(collection: string, id: string, value: any, db: Db) {
    switch (collection) {
        case "tariffs":
            await updateTariffsColors(db, id, value as Tariff);
            break;
        case "contacts":
            await updateContactInformation(db, id, value as Contact);
            break;
    }
}

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const id = req.body._id;
    let value = req.body;
    delete value._id;

    logger.info("Updating " + id + " in " + collection + " with " + JSON.stringify(value));
    const data = await db.collection(collection as string).updateOne({ _id: new ObjectId(id) }, { $set: value });

    await processSideEffects(collection as string, id, value, db);

    res.status(200).json(data);
};
