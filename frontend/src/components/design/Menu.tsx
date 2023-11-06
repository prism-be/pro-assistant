import {useTranslation} from "react-i18next";
import {useEffect, useState} from "react";
import {format} from "date-fns";
import {onToggledMobileMenu, toggledMobileMenu} from "@/libs/events/mobileMenu";
import {Link, useLocation} from "react-router-dom";

const Menu = () => {
    const { t } = useTranslation("common");
    let location = useLocation();
    const [displayMobileMenu, setDisplayMobileMenu] = useState(false);

    useEffect(() => {
        const subscription = onToggledMobileMenu().subscribe(() => {
            setDisplayMobileMenu(!displayMobileMenu);
        });
        return () => subscription.unsubscribe();
    }, [displayMobileMenu]);

    const getActiveLinkClass = (path: string) => {
        if (location.pathname.startsWith(path)) {
            return " border-r-4 border-primary bg-gray-100 ";
        }

        return "";
    };

    const itemClassName = "block px-4 py-2 hover:bg-gray-100 no-underline hover:no-underline";

    return (
        <div
            className={
                "bg-white w-64 border-r h-100 fixed top-14 h-full z-50 " + (displayMobileMenu ? "block" : "hidden md:block")
            }
            onClick={() => toggledMobileMenu()}
        >
            <ul>
                <li className={"px-4 pt-4 font-bold"}>{t("menu.global")}</li>
                <li className={getActiveLinkClass("/contacts")}>
                    <Link className={itemClassName} to={"/contacts"}>
                        {t("menu.contacts")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/agenda")}>
                    <Link className={itemClassName} to={"/agenda/" + format(new Date(), "yyyy-MM-dd")}>
                        {t("menu.agenda")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/calendar")}>
                    <Link className={itemClassName} to={"/calendar"}>
                        {t("menu.calendar")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/appointments")}>
                    <Link className={itemClassName} to={"/appointments"}>
                        {t("menu.appointments")}
                    </Link>
                </li>

                <div className={"hidden md:block"}>
                    <li className={"px-4 pt-4 font-bold"}>{t("menu.configurationTitle")}</li>
                    <li className={getActiveLinkClass("/configuration")}>
                        <Link className={itemClassName} to={"/configuration"}>
                            {t("menu.configuration")}
                        </Link>
                    </li>
                    <li className={getActiveLinkClass("/documents")}>
                        <Link className={itemClassName} to={"/documents"}>
                            {t("menu.documents")}
                        </Link>
                    </li>
                </div>

                <li className={"px-4 pt-4 font-bold"}>{t("menu.accounting")}</li>
                <li className={"hidden " + getActiveLinkClass("/accounting/forecast")}>
                    <Link className={itemClassName} to={"/accounting/forecast"}>
                        {t("menu.accounting-forecast")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/accounting/closing")}>
                    <Link className={itemClassName} to={"/accounting/closing"}>
                        {t("menu.accounting-closing")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/accounting/reporting")}>
                    <Link className={itemClassName} to={"/accounting/reporting"}>
                        {t("menu.accounting-reporting")}
                    </Link>
                </li>
                <li className={getActiveLinkClass("/accounting/documents")}>
                    <Link className={itemClassName} to={"/accounting/documents"}>
                        {t("menu.accounting-documents")}
                    </Link>
                </li>
            </ul>
        </div>
    );
};

export default Menu;
