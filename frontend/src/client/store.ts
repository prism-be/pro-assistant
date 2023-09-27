import {observable} from "@legendapp/state";
import {enableReactUse} from "@legendapp/state/config/enableReactUse";

enableReactUse();

export const currentUser$ = observable({name: "Anne Onyme", isAuthenticated: false, checked: false, redirected: false});