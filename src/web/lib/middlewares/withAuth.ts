import {NextApiRequest, NextApiResponse} from "next";
import jwksClient from 'jwks-rsa';
import jwt from 'jsonwebtoken'

export function withAuth(handler: { (req: NextApiRequest, res: NextApiResponse): void; (arg0: NextApiRequest, arg1: NextApiResponse): any; }) {
    return async (req: NextApiRequest, res: NextApiResponse) => {
        const authHeader = req.headers.authorization;
        if (!authHeader) {
            return res.status(401).end('Not authenticated. No Auth header');
        }

        const client = jwksClient({
            jwksUri: "https://" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".b2clogin.com/" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW + "/discovery/v2.0/keys"
        });

        function getKey(header: any, callback: any){
            client.getSigningKey(header.kid, function(err, key) {
                // @ts-ignore
                const signingKey = key?.publicKey || key?.rsaPublicKey;
                callback(null, signingKey);
            });
        }
        
        const options = {
            audience: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID
        };
        
        const token = req.headers.authorization?.split(' ')[1]!;

        let decodedToken;
        
        jwt.verify(token, getKey, options, function(err, decoded) {
            decodedToken = decoded;

            if (!decodedToken)
            {
                return res.status(401).end('Not authenticated. No Auth header');
            }

            return handler(req, res);
        });
    };
}