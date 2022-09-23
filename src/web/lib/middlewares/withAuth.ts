import {NextApiRequest, NextApiResponse} from "next";
const passport = require('passport');

export function withAuth(handler: { (req: NextApiRequest, res: NextApiResponse): void; (arg0: NextApiRequest, arg1: NextApiResponse): any; }) {
    return async (req: NextApiRequest, res: NextApiResponse) => {
        const authHeader = req.headers.authorization;
        if (!authHeader) {
            return res.status(401).end('Not authenticated. No Auth header');
        }

        /*const token = authHeader.split(' ')[1];
        let decodedToken;
        try {
            decodedToken = await auth.verifyIdToken(token);
            if (!decodedToken || !decodedToken.uid)
                return res.status(401).end('Not authenticated');
            req.uid = decodedToken.uid;
        } catch (error) {
            console.log(error.errorInfo);
            const errorCode = error.errorInfo.code;
            error.status = 401;
            if (errorCode === 'auth/internal-error') {
                error.status = 500;
            }
            //TODO handlle firebase admin errors in more detail
            return res.status(error.status).json({ error: errorCode });
        }*/

        return handler(req, res);
    };
}