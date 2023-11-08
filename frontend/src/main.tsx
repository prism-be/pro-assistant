import React, {Suspense} from 'react'
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

import {passThroughLoader} from "@/routing/loaders.ts";
import {SWRConfig} from "swr";
import {getData} from "@/libs/http.ts";
import {Alert} from "@/components/Alert.tsx";
import Insights from "@/components/Insights.tsx";

const Admin = React.lazy(() => import("@/pages/admin.tsx"));
const Agenda = React.lazy(() => import("@/pages/agenda.tsx"));
const AccountingClosing = React.lazy(() => import("@/pages/accounting/closing.tsx"));
const AccountingDocuments = React.lazy(() => import("@/pages/accounting/documents.tsx"));
const AccountingReporting = React.lazy(() => import("@/pages/accounting/reporting.tsx"));
const Appointments = React.lazy(() => import("@/pages/appointments.tsx"));
const Calendar = React.lazy(() => import("@/pages/calendar.tsx"));
const Contacts = React.lazy(() => import("@/pages/contacts/index.tsx"));
const ContactDetail = React.lazy(() => import("@/pages/contacts/details.tsx"));
const Configuration = React.lazy(() => import("@/pages/configuration.tsx"));
const Documents = React.lazy(() => import("@/pages/documents.tsx"));

const router = createBrowserRouter([
    {
        path: "/",
        element: <Suspense><Contacts/></Suspense>,
    },
    {
        path: "/appointments/:appointmentId",
        element: <Suspense><Appointments/></Suspense>,
        loader: passThroughLoader
    },
    {
        path: "/calendar/:date",
        element: <Suspense><Calendar/></Suspense>,
        loader: passThroughLoader
    },
    {
        path: "/accounting/closing",
        element: <Suspense><AccountingClosing/></Suspense>,
    },
    {
        path: "/accounting/documents/:year",
        element: <Suspense><AccountingDocuments/></Suspense>,
        loader: passThroughLoader
    },
    {
        path: "/accounting/reporting",
        element: <Suspense><AccountingReporting /></Suspense>
    },
    {
        path: "/admin",
        element: <Suspense><Admin/></Suspense>
    },
    {
        path: "/agenda/:date",
        element: <Suspense><Agenda/></Suspense>,
        loader: passThroughLoader
    },
    {
        path: "/contacts",
        element: <Suspense><Contacts/></Suspense>,
    },
    {
        path: "/configuration",
        element: <Suspense><Configuration/></Suspense>
    },
    {
        path: "/contacts/:contactId",
        element: <Suspense><ContactDetail/></Suspense>,
        loader: passThroughLoader
    },
    {
        path: "/documents",
        element: <Suspense><Documents/></Suspense>,
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
