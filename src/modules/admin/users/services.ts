import {getMongoClient} from "@/libs/mongodb";
import {User} from "@/modules/admin/users/types";

export async function getUser(email: string): Promise<User | undefined> {
    const mongoClient = await getMongoClient();
    const db = mongoClient.db("pro-assistant");

    const users = await db.collection<User>("users").find({email: email}).toArray();

    if (users.length === 0) {
        return undefined;
    }

    return users[0];
}

export async function saveUser(user: User) {
    const mongoClient = await getMongoClient();
    const db = mongoClient.db("pro-assistant");

    await db.collection<User>("users").insertOne(user);
}