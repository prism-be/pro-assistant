import logger from "@/libs/logging";
import { MiddlewareFactory } from "@/modules/middlewares/types";
import { NextFetchEvent, NextMiddleware, NextRequest } from "next/server";

export const withErrorHandling: MiddlewareFactory = (next: NextMiddleware) => {
    return async (request: NextRequest, _next: NextFetchEvent) => {
        try {
            return await next(request, _next);
        } catch (error) {
            logger.error(error);
            return new Response("Internal Server Error", { status: 500 });
        }
    };
};
