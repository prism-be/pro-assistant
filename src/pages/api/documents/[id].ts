import { NextApiRequest, NextApiResponse } from "next";
import { GridFSBucket, ObjectId } from "mongodb";
import { getDatabase } from "@/libs/api";

export default async (req: NextApiRequest, res: NextApiResponse) => {
    const db = await getDatabase("GET", req, res);
    const { id } = req.query;

    const bucket = new GridFSBucket(db);
    const downloadStream = bucket.openDownloadStream(new ObjectId(id as string));

    downloadStream.on("file", () => {
        res.setHeader("Content-Type", "application/pdf");
    });

    downloadStream.pipe(res);
};
