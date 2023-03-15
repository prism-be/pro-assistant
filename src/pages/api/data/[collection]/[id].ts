import { NextApiRequest, NextApiResponse } from "next";
import { ObjectId } from "mongodb";
import { getDatabaseAndCollection } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { db, collection } = await getDatabaseAndCollection("POST", req, res);

    const { id } = req.query;

    const data = await db.collection(collection as string).findOne({ _id: new ObjectId(id as string) });

    res.status(200).json(data);
};
