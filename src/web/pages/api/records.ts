import {NextApiRequest, NextApiResponse} from "next";
import {withAuth} from "../../lib/middlewares/withAuth";

const handler = (req: NextApiRequest, res: NextApiResponse) => {
    res.status(200).json({ text: 'Hello' });
}

export default withAuth(handler);