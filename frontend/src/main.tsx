import React from 'react'
import ReactDOM from 'react-dom/client'
import {MsalAuthenticationTemplate, MsalProvider} from "@azure/msal-react";
import {InteractionType} from "@azure/msal-browser";
import {msalInstance, authRequest} from "./libs/msal";
import '../i18n.ts'

import {
    createBrowserRouter,
    RouterProvider,
} from "react-router-dom";

import './index.css'

import {enableReactUse} from '@legendapp/state/config/enableReactUse';
import {enableReactComponents} from '@legendapp/state/config/enableReactComponents'

enableReactUse();
enableReactComponents();

import Contacts from "@/pages/contacts";
import ContactDetail from "@/pages/contacts/details.tsx";
import {passThroughLoader} from "@/routing/loaders.ts";
import Agenda from "@/pages/agenda";
import Admin from "@/pages/admin.tsx";
import Configuration from "@/pages/configuration.tsx";
import {SWRConfig} from "swr";
import {getData} from "@/libs/http.ts";

const router = createBrowserRouter([
    {
        path: "/",
        element: <Contacts/>,
    },
    {
        path: "/admin",
        element: <Admin/>
    },
    {
        path: "/agenda/:date",
        element: <Agenda/>,
        loader: passThroughLoader
    },
    {
        path: "/contacts",
        element: <Contacts/>,
    },
    {
        path: "/configuration",
        element: <Configuration/>
    },
    {
        path: "/contacts/:contactId",
        element: <ContactDetail/>,
        loader: passThroughLoader
    },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <SWRConfig value={{fetcher: getData}}>
            <MsalProvider instance={msalInstance}>
                <MsalAuthenticationTemplate interactionType={InteractionType.Redirect}
                                            authenticationRequest={authRequest}>
                    <RouterProvider router={router}/>
                </MsalAuthenticationTemplate>
            </MsalProvider>
        </SWRConfig>
    </React.StrictMode>,
)
