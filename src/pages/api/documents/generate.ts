import { NextApiRequest, NextApiResponse } from "next";
import { DocumentRequest } from "@/modules/documents/types";
import { generateDocument } from "@/modules/documents/generator";
import { getDatabase } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const db = await getDatabase("POST", req, res);

    const body = req.body as DocumentRequest;

    if (!body) {
        throw new Error("The body must be defined");
    }

    await generateDocument(db, body);

    res.status(200).json({ message: "ok" });
};
