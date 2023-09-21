﻿import ContentContainer from "../../components/design/ContentContainer";
import Button from "../../components/forms/Button";
import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import {useRouter} from "next/router";
import Section from "../../components/design/Section";
import {Contact} from "@/libs/models";
import {searchContacts} from "@/libs/search";
import {useMountOnce, useObservable} from "@legendapp/state/react";
import ReactiveInputText from "@/components/forms/ReactiveInputText";
import ReactiveInputDate from "@/components/forms/ReactiveInputDate";
import { usePersistedObservable } from "@legendapp/state/react-hooks/usePersistedObservable"
import { ObservablePersistSessionStorage } from '@legendapp/state/persist-plugins/local-storage'

const Contacts: NextPage = () => {
    const { t } = useTranslation("common");
    const router = useRouter();

    const contacts$ = useObservable<Contact[] | null>(null);
    const contacts = contacts$.use();
    
    const search$ = usePersistedObservable({
        lastName: "",
        firstName: "",
        phoneNumber: "",
        birthDate: "",
    }, { persistLocal: ObservablePersistSessionStorage, local: "contacts/search-contacts" });
    
    useMountOnce(() => {
        if (search$.lastName.get()?.length > 0 || search$.firstName.get()?.length > 0 || search$.phoneNumber.get()?.length > 0 || search$.birthDate.get()?.length > 0)
        {
            performSearch();
        }
    });

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement> | React.MouseEvent<any>) => {
        e.preventDefault();
        await performSearch();
        
    };
    
    async function performSearch() {
        contacts$.set(await searchContacts(search$.get()));
    }

    const navigate = async (id: string) => {
        await router.push("/contacts/" + id);
    };

    const resetSearch = () => {
        search$.set({
            lastName: "",
            firstName: "",
            phoneNumber: "",
            birthDate: ""
        });
        contacts$.set(null);
    };

    return (
        <ContentContainer>
            <>
                <Section>
                    <>
                        <h1>{t("pages.contacts.title")}</h1>
                        <form className={"grid gap-2 md:grid-cols-2 lg:grid-cols-4"} onSubmit={handleSubmit}>
                            <div className={"pt-2"}>
                                <ReactiveInputText label={t("fields.lastName")} value={search$.lastName} autoCapitalize={true} />
                            </div>
                            <div className={"pt-2"}>
                                <ReactiveInputText label={t("fields.firstName")} value={search$.firstName} autoCapitalize={true} />
                            </div>
                            <div className={"pt-2"}>
                                <ReactiveInputText label={t("fields.phoneNumber")} value={search$.phoneNumber} autoCapitalize={true} />
                            </div>
                            <div className={"pt-2"}>
                                <ReactiveInputDate label={t("fields.birthDate")} value={search$.birthDate} />
                            </div>
                            <div className={"pt-2"}>
                                <Button text={t("actions.reset")} onClick={resetSearch} secondary={true} />
                            </div>
                            <div className={"pt-2"}>
                                <Button
                                    text={t("actions.new")}
                                    onClick={() => router.push("/contacts/000000000000000000000000")}
                                    secondary={true}
                                />
                            </div>
                            <div className={"pt-2 md:col-start-2 lg:col-start-4"}>
                                <Button submit={true} text={t("actions.search")} onClick={handleSubmit} />
                            </div>
                        </form>
                    </>
                </Section>

                <Section>
                    <>
                        {contacts && (
                            <>
                                <h2>{t("results.title")}</h2>
                                {contacts.length !== 0 && (
                                    <div>
                                        <div className={"grid grid-cols-2 lg:grid-cols-4 border-b border-primary"}>
                                            <div className={"font-bold p-2"}>{t("fields.lastName")}</div>
                                            <div className={"font-bold p-2"}>{t("fields.firstName")}</div>
                                            <div className={"font-bold p-2"}>{t("fields.phoneNumber")}</div>
                                            <div className={"font-bold p-2"}>{t("fields.birthDate")}</div>
                                        </div>
                                        {contacts?.map((contact) => (
                                            <div
                                                className={
                                                    "grid grid-cols-2 lg:grid-cols-4 border-b border-dashed last:border-0 cursor-pointer hover:bg-gray-100"
                                                }
                                                key={contact.id}
                                                onClick={() => navigate(contact.id)}
                                            >
                                                <div className={"p-2"}>{contact.lastName}</div>
                                                <div className={"p-2"}>{contact.firstName}</div>
                                                <div className={"p-2"}>{contact.phoneNumber}</div>
                                                <div className={"p-2"}>{contact.birthDate}</div>
                                            </div>
                                        ))}
                                    </div>
                                )}

                                {contacts.length === 0 && (
                                    <div className={"styles.helpText"}>{t("results.noResults")}</div>
                                )}
                            </>
                        )}

                        {!contacts && (
                            <>
                                <div className={"text-center italic"}>{t("results.doSearch")}</div>
                            </>
                        )}
                    </>
                </Section>
            </>
        </ContentContainer>
    );
};

export default Contacts;
