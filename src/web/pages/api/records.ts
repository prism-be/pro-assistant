import {NextApiRequest, NextApiResponse} from "next";
import {withAuth} from "../../lib/middlewares/withAuth";

export const handleGetRecords = (req: NextApiRequest, res: NextApiResponse) => {

    if (req.method !== 'GET') {
        res.status(405).json({ err: 'Method not allowed' });
        return;
    }
    
    res.status(200).json({ text: 'Hello' });
}

export default withAuth(handleGetRecords);