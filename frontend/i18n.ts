import i18n from "i18next";
import {initReactI18next} from "react-i18next";

// @ts-ignore
import accountingFR from './locales/fr/accounting.json';
// @ts-ignore
import commonFR from './locales/fr/common.json';
// @ts-ignore
import configurationFR from './locales/fr/configuration.json';
// @ts-ignore
import documentsFR from './locales/fr/documents.json';
// @ts-ignore
import loginFR from './locales/fr/login.json';

i18n.use(initReactI18next).init({
    resources: {
        fr: {
            accounting: accountingFR,
            common: commonFR,
            configuration: configurationFR,
            documents: documentsFR,
            login: loginFR
        }
    },
    lng: "fr",
});