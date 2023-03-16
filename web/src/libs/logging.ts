import winston from "winston";
// @ts-ignore
import { AzureApplicationInsightsLogger } from "winston-azure-application-insights";

const logger = winston.createLogger({
    level: "info",
    format: winston.format.json(),
    transports: [new winston.transports.File({ filename: "error.log", level: "error" }), new winston.transports.File({ filename: "combined.log" })],
});

logger.add(
    new winston.transports.Console({
        format: winston.format.simple(),
    })
);

if (process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING && process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING.length > 0) {
    logger.add(
        new AzureApplicationInsightsLogger({
            connectionString: process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING,
            level: "info",
            context: {
                tags: {
                    "ai.cloud.role": "pro-assistant",
                },
            },
        })
    );
}

export default logger;
