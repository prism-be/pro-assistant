import { NextApiRequest, NextApiResponse } from "next";
import logger from "@/libs/logging";
import { getDatabaseAndCollection } from "@/libs/api";
import { processSideEffects } from "@/modules/data/effects";
import { processRequirements } from "@/modules/data/requirements";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const value = req.body;
    delete value._id;

    logger.info("Insert a new item in " + collection + " with " + JSON.stringify(value));

    await processRequirements(collection as string, value._id, value, db);
    const data = await db.collection(collection as string).insertOne(value);
    await processSideEffects(collection as string, data.insertedId.toHexString(), value, db);

    res.status(200).json(data);
};
