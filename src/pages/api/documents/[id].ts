import { NextApiRequest, NextApiResponse } from "next";
import { getSession } from "@auth0/nextjs-auth0";
import { getUserDatabase } from "@/libs/mongodb";
import { GridFSBucket, ObjectId } from "mongodb";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const { id } = req.query;

    const session = await getSession(req, res);

    if (!session?.user) {
        res.status(401).json({ message: "Unauthorized" });
        return;
    }

    const db = await getUserDatabase(session.user.email);
    const bucket = new GridFSBucket(db);
    const downloadStream = bucket.openDownloadStream(new ObjectId(id as string));

    downloadStream.on("file", () => {
        res.setHeader("Content-Type", "application/pdf");
    });

    downloadStream.pipe(res);
};
