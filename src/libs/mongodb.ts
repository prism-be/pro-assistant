import { Db, MongoClient, ObjectId } from "mongodb";
import { getUser, saveUser } from "@/modules/admin/users/services";
import logger from "@/libs/logging";
import { cache } from "@/libs/cache";
import { User } from "@/modules/admin/users/types";

let currentClient: MongoClient | undefined;

export async function getMongoClient() {
  if (currentClient) {
    return currentClient;
  }

  if (!process.env.MONGODB_CONNECTION_STRING) {
    throw new Error("The MONGODB_CONNECTION_STRING is empty");
  }

  currentClient = new MongoClient(process.env.MONGODB_CONNECTION_STRING);
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

  let db = cache.get<Db>("db-" + user.organization);

  if (db) {
    return db;
  }

  const client = await getMongoClient();
  db = client.db(user.organization);

  //cache.set("db-" + user.organization, db, 60 * 60);

  return db;
}
