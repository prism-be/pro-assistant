import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import ContentContainer from "../../components/design/ContentContainer";
import {useRouter} from "next/router";
import useSWR, {mutate} from "swr";
import InputText from "../../components/forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect} from "react";
import Button from "../../components/forms/Button";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import InputDate from "../../components/forms/InputDate";
import Section from "../../components/design/Section";
import {ContactAppointments} from "@/components/contacts/ContactAppointments";
import {ArrowSmallLeftIcon} from "@heroicons/react/24/outline";
import {getData, postData} from "@/libs/http";
import {Contact, UpsertResult} from "@/libs/models";
import {alertSuccess} from "@/libs/events/alert";

const Contacts: NextPage = () => {
    const { t } = useTranslation("common");
    const router = useRouter();
    const { cid } = router.query;

    const loadContact = async (route: string) => {
        if (route === "/data/contacts/000000000000000000000000") {
            return null;
        }

        return await getData<Contact>(route);
    };

    const { data: contact, mutate: mutateContact } = useSWR("/data/contacts/" + cid, loadContact);
    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        getValues,
    } = useForm();

    useEffect(() => {
        if (contact) {
            const d: any = contact;
            Object.getOwnPropertyNames(contact).forEach((field) => {
                setValue(field, d[field]);
            });
        }
    }, [contact, setValue]);

    const saveContactForm = async (data: any) => {
        if (cid === "000000000000000000000000") {
            data.id = "";
            const result = await postData<UpsertResult>("/data/contacts/insert", data);
            alertSuccess(t("details.saveSuccess"), { autoClose: true });
            await router.push("/contacts/" + result.id);
            return;
        }

        await postData("/data/contacts/update", data);
        await mutateContact();
        await mutate(`/api/contacts/${cid}/appointments`);
        alertSuccess(t("pages.contacts.details.saveSuccess"), { autoClose: true });
    };

    const onSaveContactSubmit = async (data: any) => {
        await saveContactForm(data);
    };

    const [isSavePressed, isSaveEvent] = useKeyboardJs("ctrl + s");
    useEffect(() => {
        if (isSavePressed) {
            isSaveEvent?.preventDefault();
            const data = getValues();
            saveContactForm(data);
        }
    }, [isSavePressed, getValues]);

    return (
        <ContentContainer>
            <Section>
                <div className={"flex"}>
                    <a className={"w-8 cursor-pointer"} onClick={() => router.push("/contacts")}>
                        <ArrowSmallLeftIcon />
                    </a>
                    <h1>
                        {t("pages.contacts.details.title")} {contact?.lastName} {contact?.firstName}
                    </h1>
                </div>
                <form
                    className={"grid gap-2 md:grid-cols-6 lg:grid-cols-12"}
                    onSubmit={handleSubmit(onSaveContactSubmit)}
                >
                    <div className={"md:col-span-2 lg:col-span-1"}>
                        <InputText
                            name="title"
                            label={t("fields.title")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.lastName}
                            autoCapitalize={true}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-4"}>
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
                    <div className={"md:col-span-2 lg:col-span-4"}>
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
                    <div className={"md:col-span-2 lg:col-span-3"}>
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
                    <div className={"md:col-span-2 lg:col-span-6"}>
                        <InputText
                            name="email"
                            label={t("fields.email")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.email}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-6"}>
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
                    <div className={"md:col-span-4 lg:col-span-10"}>
                        <InputText
                            name="street"
                            label={t("fields.street")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.street}
                            autoCapitalize={true}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-2"}>
                        <InputText
                            name="number"
                            label={t("fields.number")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.number}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-2"}>
                        <InputText
                            name="zipCode"
                            label={t("fields.zipCode")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.zipCode}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-5"}>
                        <InputText
                            name="city"
                            label={t("fields.city")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.city}
                            autoCapitalize={true}
                        />
                    </div>
                    <div className={"md:col-span-2 lg:col-span-5"}>
                        <InputText
                            name="country"
                            label={t("fields.country")}
                            type="text"
                            required={false}
                            register={register}
                            setValue={setValue}
                            error={errors.country}
                            autoCapitalize={true}
                        />
                    </div>
                    <div className={"md:col-start-4 pt-4 md:pt-0 md:col-span-3 lg:col-span-3 lg:col-start-10"}>
                        <Button text={t("actions.save")} onClick={handleSubmit(onSaveContactSubmit)} secondary={true} />
                    </div>
                </form>
            </Section>
            <ContactAppointments contactId={cid as string} />
        </ContentContainer>
    );
};

export default Contacts;
