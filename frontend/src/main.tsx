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
import Agenda from "@/pages/agenda.tsx";
import Admin from "@/pages/admin.tsx";
import Configuration from "@/pages/configuration.tsx";
import {SWRConfig} from "swr";
import {getData} from "@/libs/http.ts";
import Documents from "@/pages/documents.tsx";
import AccountingClosing from "@/pages/accounting/closing.tsx";
import AccountingReporting from "@/pages/accounting/reporting.tsx";
import AccountingDocuments from "@/pages/accounting/documents.tsx";
import Calendar from "@/pages/calendar.tsx";
import Appointments from "@/pages/appointments.tsx";
import {Alert} from "@/components/Alert.tsx";
import Insights from "@/components/Insights.tsx";

const router = createBrowserRouter([
    {
        path: "/",
        element: <Contacts/>,
    },
    {
        path: "/appointments/:appointmentId",
        element: <Appointments/>,
        loader: passThroughLoader
    },
    {
        path: "/calendar/:date",
        element: <Calendar/>,
        loader: passThroughLoader
    },
    {
        path: "/accounting/closing",
        element: <AccountingClosing/>,
    },
    {
        path: "/accounting/documents/:year",
        element: <AccountingDocuments/>,
        loader: passThroughLoader
    },
    {
        path: "/accounting/reporting",
        element: <AccountingReporting/>,
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
    {
        path: "/documents",
        element: <Documents/>,
    },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <Insights>
            <SWRConfig value={{fetcher: getData}}>
                <MsalProvider instance={msalInstance}>
                    <MsalAuthenticationTemplate interactionType={InteractionType.Redirect}
                                                authenticationRequest={authRequest}>
                        <RouterProvider router={router}/>
                        <Alert/>
                    </MsalAuthenticationTemplate>
                </MsalProvider>
            </SWRConfig>
        </Insights>
    </React.StrictMode>,
)
