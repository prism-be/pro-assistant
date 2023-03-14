import {NextApiRequest, NextApiResponse} from "next";
import {getUserDatabase} from "@/libs/mongodb";
import {getSession} from "@auth0/nextjs-auth0";
import {ObjectId} from "mongodb";
import logger from "@/libs/logging";

export const config = {
    api: {
        bodyParser: {
            sizeLimit: '50mb'
        }
    }
}

export default async (req: NextApiRequest, res: NextApiResponse) => {
    if (req.method !== 'POST') {
        res.status(405).send({message: 'Only POST requests allowed'})
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

    let values = req.body as any[];

    const results = [];
    for (let value of values) {

        const id = value._id;
        delete value._id;
        logger.info("Updating " + id + " in " + collection);
        const data = await db.collection(collection as string).updateOne({_id: id}, {$set: value});
        results.push(data);

        if (data.matchedCount === 0) {
            logger.info("Inserting " + id + " in " + collection);
            const data = await db.collection(collection as string).insertOne(value);
            results.push(data);
        }
    }
    
    res.status(200).json(results);
}