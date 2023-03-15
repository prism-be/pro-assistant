import { NextApiRequest, NextApiResponse } from "next";
import logger from "@/libs/logging";
import { getDatabase, getDatabaseAndCollection } from "@/libs/api";

export const config = {
    api: {
        bodyParser: {
            sizeLimit: "50mb",
        },
    },
};

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    let values = req.body as any[];

    const results = [];
    for (let value of values) {
        const id = value._id;
        delete value._id;
        logger.info("Updating " + id + " in " + collection);
        const data = await db.collection(collection as string).updateOne({ _id: id }, { $set: value });
        results.push(data);

        if (data.matchedCount === 0) {
            logger.info("Inserting " + id + " in " + collection);
            const data = await db.collection(collection as string).insertOne(value);
            results.push(data);
        }
    }

    res.status(200).json(results);
};
