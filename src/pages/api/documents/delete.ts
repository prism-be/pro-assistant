import { DocumentRequest } from "@/modules/documents/types";
import { deleteDocument } from "@/modules/documents/generator";
import { NextApiRequest, NextApiResponse } from "next";
import { getDatabase } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const db = await getDatabase("DELETE", req, res);

    const body = req.body as DocumentRequest;

    if (!body) {
        throw new Error("The body must be defined");
    }

    await deleteDocument(db, body);

    res.status(200).json({ message: "ok" });
};
