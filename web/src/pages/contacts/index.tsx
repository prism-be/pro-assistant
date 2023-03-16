import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import InputDate from "../../components/forms/InputDate";
import Button from "../../components/forms/Button";
import { NextPage } from "next";
import useTranslation from "next-translate/useTranslation";
import { useForm } from "react-hook-form";
import { useEffect, useState } from "react";
import { useRouter } from "next/router";
import Section from "../../components/design/Section";
import { Contact } from "@/libs/models";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import { searchContacts } from "@/modules/contacts/search";

const Contacts: NextPage = () => {
    const { t } = useTranslation("common");
    const router = useRouter();

    const [contacts, setContacts] = useState<Contact[] | null>(null);

    const schema = yup.object({}).required();

    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        setFocus,
        reset,
    } = useForm({ resolver: yupResolver(schema) });

    useEffect(() => {
        const sessionSearchContacts = sessionStorage.getItem("contacts/search-contacts");
        if (sessionSearchContacts) {
            const data = JSON.parse(sessionSearchContacts);
            Object.getOwnPropertyNames(data).forEach((field) => {
                setValue(field, data[field]);
            });
            onSubmit(data);
        }
        setFocus("lastName");
    }, [setFocus]);

    const onSubmit = async (data: any) => {
        sessionStorage.setItem("contacts/search-contacts", JSON.stringify(data));

        setContacts(await searchContacts(data));
    };

    const navigate = async (id: string) => {
        await router.push("/contacts/" + id);
    };

    const resetSearch = () => {
        sessionStorage.removeItem("contacts/search-contacts");
        reset();
        setContacts(null);
        setFocus("lastName");
    };

    const [isNewPressed, isNewPressedEvent] = useKeyboardJs("alt + n");
    useEffect(() => {
        if (isNewPressed) {
            isNewPressedEvent?.preventDefault();
            router.push("/contacts/000000000000000000000000");
        }
    }, [isNewPressed, router]);

    return (
        <ContentContainer>
            <>
                <Section>
                    <>
                        <h1>{t("pages.contacts.title")}</h1>
                        <form className={"grid gap-2 md:grid-cols-2 lg:grid-cols-4"} onSubmit={handleSubmit(onSubmit)}>
                            <div className={"pt-2"}>
                                <InputText
                                    name="lastName"
                                    label={t("fields.lastName")}
                                    type="text"
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.lastName}
                                    autoCapitalize={true}
                                />
                            </div>
                            <div className={"pt-2"}>
                                <InputText
                                    name="firstName"
                                    label={t("fields.firstName")}
                                    type="text"
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.firstName}
                                    autoCapitalize={true}
                                />
                            </div>
                            <div className={"pt-2"}>
                                <InputText
                                    name="phoneNumber"
                                    label={t("fields.phoneNumber")}
                                    type="text"
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.phoneNumber}
                                />
                            </div>
                            <div className={"pt-2"}>
                                <InputDate
                                    name="birthDate"
                                    label={t("fields.birthDate")}
                                    type="text"
                                    required={false}
                                    register={register}
                                    setValue={setValue}
                                    error={errors.birthDate}
                                />
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
                                <Button submit={true} text={t("actions.search")} onClick={handleSubmit(onSubmit)} />
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
