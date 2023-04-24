﻿import {fr} from "date-fns/locale";
import {languages, namespaces} from "next-i18next-static-site";

export const getLocale = () => {
    return fr;
};

const locales: any = {};
languages.map((language) => {
    locales[language] = {};

    namespaces.map((namespace) => {
        locales[language][namespace] = require("./../../locales/" +
            language +
            "/" +
            namespace +
            ".json");
    });
});

export default locales;