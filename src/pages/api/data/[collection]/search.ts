import { NextApiRequest, NextApiResponse } from "next";
import { getDatabase, getDatabaseAndCollection } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const data = await db
        .collection(collection as string)
        .find(req.body)
        .toArray();

    res.status(200).json(data);
};
