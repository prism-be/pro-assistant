import { getUserDatabase } from "@/libs/mongodb";
import { NextApiRequest, NextApiResponse } from "next";
import { getSession } from "@auth0/nextjs-auth0";
import { Db } from "mongodb";

export async function getDatabase(method: string, req: NextApiRequest, res: NextApiResponse): Promise<Db> {
    if (req.method !== method) {
        res.status(405).send({ message: `Only ${method} requests allowed` });
        throw new Error(`Only ${method} requests allowed`);
    }

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({ message: "Unauthorized" });
        throw new Error("Unauthorized");
    }

    return await getUserDatabase(session.user.email);
}

export async function getDatabaseAndCollection(method: string, req: NextApiRequest, res: NextApiResponse): Promise<{ db: Db; collection: string }> {
    if (req.method !== method) {
        res.status(405).send({ message: `Only ${method} requests allowed` });
        throw new Error(`Only ${method} requests allowed`);
    }

    let { collection } = req.query;
    collection = collection?.toString();

    if (!collection) {
        throw new Error("The collection must be defined");
    }

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({ message: "Unauthorized" });
        throw new Error("Unauthorized");
    }

    return { db: await getUserDatabase(session.user.email), collection };
}
