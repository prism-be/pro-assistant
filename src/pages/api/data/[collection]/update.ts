import {NextApiRequest, NextApiResponse} from "next";
import {getUserDatabase} from "@/libs/mongodb";
import {getSession} from "@auth0/nextjs-auth0";
import {ObjectId} from "mongodb";
import logger from "@/libs/logging";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    if (req.method !== 'POST') {
        res.status(405).send({ message: 'Only POST requests allowed' })
        return
    }

    const {collection} = req.query;

    if (!collection) {
        throw new Error("The collection must be defined")
    }

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({message: "Unauthorized"});
        return;
    }

    const db = await getUserDatabase(session.user.email);
    
    const id = req.body._id;
    let value = req.body;
    delete value._id;

    logger.info("Updating " + id + " in " + collection + " with " + JSON.stringify(value));
    const data = await db.collection(collection as string).updateOne({ _id: new ObjectId(id) }, { $set: value });

    res.status(200).json(data);
}