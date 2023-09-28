import {observable} from "@legendapp/state";
import {enableReactUse} from "@legendapp/state/config/enableReactUse";
import {enableReactComponents} from "@legendapp/state/config/enableReactComponents";

enableReactUse();
enableReactComponents()

export const currentUser$ = observable({name: "Anne Onyme", isAuthenticated: false, checked: false, redirected: false});