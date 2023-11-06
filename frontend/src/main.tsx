import React from 'react'
import ReactDOM from 'react-dom/client'
import {MsalAuthenticationTemplate, MsalProvider} from "@azure/msal-react";
import {HelmetProvider} from 'react-helmet-async';
import {InteractionType} from "@azure/msal-browser";
import {msalInstance, authRequest} from "./libs/msal";
import '../i18n.ts'

import {
    createBrowserRouter,
    RouterProvider,
} from "react-router-dom";

import './index.css'

import {enableReactUse} from '@legendapp/state/config/enableReactUse';
import { enableReactComponents } from '@legendapp/state/config/enableReactComponents'
enableReactUse();
enableReactComponents();

import Contacts from "@/pages/contacts";

const router = createBrowserRouter([
    {
        path: "/",
        element: <Contacts />,
    },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <HelmetProvider>
            <MsalProvider instance={msalInstance}>
                <MsalAuthenticationTemplate interactionType={InteractionType.Redirect}
                                            authenticationRequest={authRequest}>
                    <RouterProvider router={router}/>
                </MsalAuthenticationTemplate>
            </MsalProvider>
        </HelmetProvider>
    </React.StrictMode>,
)
