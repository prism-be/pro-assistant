/**
 * @jest-environment node
 */

import { createMocks, RequestMethod } from 'node-mocks-http';
import {handleGetRecords} from "../../pages/api/records";
import {NextApiRequest, NextApiResponse} from "next";

describe('/api/records', () => {
    function mockRequestResponse(method: RequestMethod = 'GET') {
        // @ts-ignore
        const {
            req,
            res,
        }: { req: NextApiRequest; res: NextApiResponse } = createMocks({ method });
        req.headers = {
            'Content-Type': 'application/json',
        };
        return { req, res };
    }

    it('should return a 200 if HTTP method is GET', async () => {
        const { req, res } = mockRequestResponse('GET'); 

        await handleGetRecords(req, res);

        expect(res.statusCode).toBe(200);
        // @ts-ignore
        expect(res._getJSONData()).not.toBeNull();
    });
    
    
    it('should return a 405 if HTTP method is not GET', async () => {
        const { req, res } = mockRequestResponse('POST'); // Invalid HTTP call

        await handleGetRecords(req, res);

        expect(res.statusCode).toBe(405);
        // @ts-ignore
        expect(res._getJSONData()).toEqual({
            err: 'Method not allowed',
        });
    });
    
    });