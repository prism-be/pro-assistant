import {Subject} from "rxjs";

const toggleMobileMenu = new Subject();

export const onToggledMobileMenu = () => {
    return toggleMobileMenu.asObservable();
};

export const toggledMobileMenu = () => {
    toggleMobileMenu.next({});
};
