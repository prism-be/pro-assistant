import * as msal from "@azure/msal-browser";

const userFlow = import.meta.env.VITE_PUBLIC_AZURE_AD_USER_FLOW!;
const tenantName = import.meta.env.VITE_PUBLIC_AZURE_AD_TENANT_NAME!;
const clientId = import.meta.env.VITE_PUBLIC_AZURE_AD_CLIENT_ID!;

export const b2cPolicies = {
    names: {
        signUpSignIn: userFlow,
        forgotPassword: userFlow,
        editProfile: userFlow
    },
    authorities: {
        signUpSignIn: {
            authority: "https://" + tenantName + ".b2clogin.com/" + tenantName + ".onmicrosoft.com/" + userFlow,
        },
        forgotPassword: {
            authority: "https://" + tenantName + ".b2clogin.com/" + tenantName + ".onmicrosoft.com/" + userFlow,
        },
        editProfile: {
            authority: "https://" + tenantName + ".b2clogin.com/" + tenantName + ".onmicrosoft.com/" + userFlow
        }
    },
    authorityDomain: tenantName + ".b2clogin.com"
}

export const msalConfig = {
    auth: {
        clientId: clientId,
        authority: b2cPolicies.authorities.signUpSignIn.authority,
        knownAuthorities: [b2cPolicies.authorityDomain],
        redirectUri: "/",
        postLogoutRedirectUri: "/",
        navigateToLoginRequestUrl: false,
    },
    cache: {
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: false,
    }
};

export const msalInstance = new msal.PublicClientApplication(msalConfig);

export const authRequest = {
    scopes: ["openid", "profile"]
};