import * as msal from "@azure/msal-browser";
import {applicationConfiguration} from "./configuration.ts";
import {currentUser$} from "../client/store.ts";

export const b2cPolicies = {
    names: {
        signUpSignIn: applicationConfiguration.userFlow,
        forgotPassword: applicationConfiguration.userFlow,
        editProfile: applicationConfiguration.userFlow
    },
    authorities: {
        signUpSignIn: {
            authority: "https://" + applicationConfiguration.tenantName + ".b2clogin.com/" + applicationConfiguration.tenantName + ".onmicrosoft.com/" + applicationConfiguration.userFlow,
        },
        forgotPassword: {
            authority: "https://" + applicationConfiguration.tenantName + ".b2clogin.com/" + applicationConfiguration.tenantName + ".onmicrosoft.com/" + applicationConfiguration.userFlow,
        },
        editProfile: {
            authority: "https://" + applicationConfiguration.tenantName + ".b2clogin.com/" + applicationConfiguration.tenantName + ".onmicrosoft.com/" + applicationConfiguration.userFlow
        }
    },
    authorityDomain: applicationConfiguration.tenantName + ".b2clogin.com"
}

export const msalConfig = {
    auth: {
        clientId: applicationConfiguration.clientId,
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

await msalInstance.initialize();

msalInstance.handleRedirectPromise().then((tokenResponse) => {
    if (tokenResponse) {
        currentUser$.redirected.set(true);
    }
});

export {msalInstance}

export const authRequest = {
    scopes: ["openid", "profile"]
};