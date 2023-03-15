import { NextApiRequest, NextApiResponse } from "next";
import { getUserDatabase } from "@/libs/mongodb";
import { getSession } from "@auth0/nextjs-auth0";
import logger from "@/libs/logging";
import { getDatabase, getDatabaseAndCollection } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const value = req.body;
    delete value._id;

    logger.info("Insert a new item in " + collection + " with " + JSON.stringify(value));
    const data = await db.collection(collection as string).insertOne(value);

    res.status(200).json(data);
};
