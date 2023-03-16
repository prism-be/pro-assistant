import { NextApiRequest, NextApiResponse } from "next";
import { getDatabaseAndCollection } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("GET", req, res);

    const data = await db
        .collection(collection as string)
        .find()
        .toArray();

    res.status(200).json(data);
};
