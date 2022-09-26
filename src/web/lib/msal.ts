import * as msal from "@azure/msal-browser";


export const b2cPolicies = {
    names: {
        signUpSignIn: process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW,
        forgotPassword: process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW,
        editProfile: process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW
    },
    authorities: {
        signUpSignIn: {
            authority: "https://" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".b2clogin.com/" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW,
        },
        forgotPassword: {
            authority: "https://" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".b2clogin.com/" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW,
        },
        editProfile: {
            authority: "https://" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".b2clogin.com/" + process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".onmicrosoft.com/" + process.env.NEXT_PUBLIC_AZURE_AD_USER_FLOW
        }
    },
    authorityDomain: process.env.NEXT_PUBLIC_AZURE_AD_TENANT_NAME + ".b2clogin.com"
}

export const msalConfig = {
    auth: {
        clientId: process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID!,
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

const msalInstance = new msal.PublicClientApplication(msalConfig);

export { msalInstance }