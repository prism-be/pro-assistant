import * as msal from "@azure/msal-browser";

import getConfig from 'next/config'
const { publicRuntimeConfig: config } = getConfig()

export const b2cPolicies = {
    names: {
        signUpSignIn: config.userFlow,
        forgotPassword: config.userFlow,
        editProfile: config.userFlow
    },
    authorities: {
        signUpSignIn: {
            authority: "https://" + config.tenantName + ".b2clogin.com/" + config.tenantName + ".onmicrosoft.com/" + config.userFlow,
        },
        forgotPassword: {
            authority: "https://" + config.tenantName + ".b2clogin.com/" + config.tenantName + ".onmicrosoft.com/" + config.userFlow,
        },
        editProfile: {
            authority: "https://" + config.tenantName + ".b2clogin.com/" + config.tenantName + ".onmicrosoft.com/" + config.userFlow
        }
    },
    authorityDomain: config.tenantName + ".b2clogin.com"
}

export const msalConfig = {
    auth: {
        clientId: config.clientId,
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