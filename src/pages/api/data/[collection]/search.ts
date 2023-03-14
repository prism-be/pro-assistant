import {NextApiRequest, NextApiResponse} from "next";
import {getUserDatabase} from "@/libs/mongodb";
import {getSession} from "@auth0/nextjs-auth0";
import {ObjectId} from "mongodb";

function buildQuery(body: any) {
    if (!body) {
        return {};
    }
    
    let query = body;
    
    // if (query.contactId && query.contactId.length === 24) {
    //     query.contactId = new ObjectId(query.contactId);
    // }
    
    return query;
}

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
    
    const query = buildQuery(req.body);
    
    const data = await db.collection(collection as string).find(query).toArray();

    res.status(200).json(data);
}