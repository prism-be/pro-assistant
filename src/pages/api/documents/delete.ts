import { DocumentRequest } from "@/modules/documents/types";
import { getUserDatabase } from "@/libs/mongodb";
import { deleteDocument } from "@/modules/documents/generator";
import { NextApiRequest, NextApiResponse } from "next";
import { getSession } from "@auth0/nextjs-auth0";

export default async (req: NextApiRequest, res: NextApiResponse) => {
  if (req.method !== "DELETE") {
    res.status(405).send({ message: "Only DELETE requests allowed" });
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

  await deleteDocument(db, body);

  res.status(200).json({ message: "ok" });
};
