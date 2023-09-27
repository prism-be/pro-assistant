import {notNullOrEmpty} from "./guards.ts";

export const applicationConfiguration = {
    userFlow: notNullOrEmpty(import.meta.env.PUBLIC_AZURE_AD_USER_FLOW, "env.PUBLIC_AZURE_AD_USER_FLOW"),
    tenantName: notNullOrEmpty(import.meta.env.PUBLIC_AZURE_AD_TENANT_NAME, "env.PUBLIC_AZURE_AD_TENANT_NAME"),
    clientId: notNullOrEmpty(import.meta.env.PUBLIC_AZURE_AD_CLIENT_ID, "env.PUBLIC_AZURE_AD_CLIENT_ID")
}