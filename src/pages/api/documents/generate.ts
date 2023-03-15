import { getUserDatabase } from "@/libs/mongodb";
import { NextApiRequest, NextApiResponse } from "next";
import { getSession } from "@auth0/nextjs-auth0";
import { DocumentRequest } from "@/modules/documents/types";
import { generateDocument } from "@/modules/documents/generator";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    if (req.method !== "POST") {
        res.status(405).send({ message: "Only POST requests allowed" });
        return;
    }

    const body = req.body as DocumentRequest;

    if (!body) {
        throw new Error("The body must be defined");
    }

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({ message: "Unauthorized" });
        return;
    }

    const db = await getUserDatabase(session.user.email);

    await generateDocument(db, body);

    res.status(200).json({ message: "ok" });
};
