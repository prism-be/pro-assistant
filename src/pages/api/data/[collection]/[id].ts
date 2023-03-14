import { NextApiRequest, NextApiResponse } from "next";
import { getUserDatabase } from "@/libs/mongodb";
import { getSession } from "@auth0/nextjs-auth0";
import { ObjectId } from "mongodb";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { collection, id } = req.query;

    if (!collection) {
        throw new Error("The collection must be defined");
    }

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({ message: "Unauthorized" });
        return;
    }

    const db = await getUserDatabase(session.user.email);
    const data = await db.collection(collection as string).findOne({ _id: new ObjectId(id as string) });

    res.status(200).json(data);
};
