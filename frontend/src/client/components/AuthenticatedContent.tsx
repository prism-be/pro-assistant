import React from "react";
import {currentUser$} from "../store.ts";

interface Props {
    children: React.JSX.Element | React.JSX.Element[];
}

export default function AuthenticatedContent(props: Props) {
    const authenticated = currentUser$.isAuthenticated.use();

    return <>
        {authenticated && props.children}
    </>;
}