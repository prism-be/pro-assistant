﻿import { MongoClient, ObjectId } from "mongodb";
import { getUser, saveUser } from "@/modules/admin/users/services";
import logger from "@/libs/logging";
import { cache } from "@/libs/cache";
import { User } from "@/modules/admin/users/types";

export async function getMongoClient() {
    if (!process.env.MONGODB_CONNECTION_STRING) {
        throw new Error("The MONGODB_CONNECTION_STRING is empty");
    }

    const currentClient = new MongoClient(process.env.MONGODB_CONNECTION_STRING);
    await currentClient.connect();
    return currentClient;
}

export async function getUserDatabase(email: string) {
    let user = cache.get<User>("user-" + email);

    if (!user) {
        logger.info(`Getting user ${email} from admin database instead of cache`);

        user = await getUser(email);

        if (!user) {
            logger.info(`Creating user ${email} in admin database`);
            user = {
                _id: new ObjectId(),
                email: email,
                organization: "demo",
            };

            await saveUser(user);
        }

        cache.set("user-" + email, user, 60 * 60);
    }

    const client = await getMongoClient();
    return client.db(user.organization);
}
