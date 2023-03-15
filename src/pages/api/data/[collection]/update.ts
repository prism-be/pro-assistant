import { NextApiRequest, NextApiResponse } from "next";
import { ObjectId } from "mongodb";
import logger from "@/libs/logging";
import { getDatabaseAndCollection } from "@/libs/api";
import { processSideEffects } from "@/modules/data/effects";
import { processRequirements } from "@/modules/data/requirements";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const id = req.body._id;
    let value = req.body;
    delete value._id;

    logger.info("Updating " + id + " in " + collection + " with " + JSON.stringify(value));

    await processRequirements(collection as string, id, value, db);
    const data = await db.collection(collection as string).updateOne({ _id: new ObjectId(id) }, { $set: value });
    await processSideEffects(collection as string, id, value, db);

    res.status(200).json(data);
};
