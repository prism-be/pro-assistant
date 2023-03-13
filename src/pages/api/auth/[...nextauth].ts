import NextAuth from "next-auth";
import AzureADB2CProvider from "next-auth/providers/azure-ad-b2c";

export const authOptions = {
    // Configure one or more authentication providers
    providers: [
        AzureADB2CProvider({
            tenantId: process.env.AZURE_AD_B2C_TENANT_NAME,
            clientId: process.env.AZURE_AD_B2C_CLIENT_ID ?? '',
            clientSecret: process.env.AZURE_AD_B2C_CLIENT_SECRET ?? '',
            primaryUserFlow: process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW,
            authorization: { params: { scope: "offline_access openid" } },
            profile(profile) {
                console.log(profile);
                return {
                    id: profile.sub,
                    organisation: profile.extension_Organization,
                    name: profile.name,
                    email: profile.emails[0],
                }
            }
        }),
    ],
};
export default NextAuth(authOptions);