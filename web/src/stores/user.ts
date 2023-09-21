import {observable} from "@legendapp/state";
import {UserInformation} from "@/libs/models";

export const currentUser$ = observable<UserInformation>();