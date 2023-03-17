import { NextPage } from "next";
import ContentContainer from "../../components/design/ContentContainer";
import Section from "../../components/design/Section";
import useTranslation from "next-translate/useTranslation";
import { useRouter } from "next/router";
import useSWR from "swr";
import { useForm } from "react-hook-form";
import { useCallback, useEffect, useState } from "react";
import InputText from "../../components/forms/InputText";
import InputSelect from "../../components/forms/InputSelect";
import { add, format, formatISO, parse, parseISO, startOfHour } from "date-fns";
import Button from "../../components/forms/Button";
import InputDate from "../../components/forms/InputDate";
import { Appointment, Contact, Tariff } from "@/libs/models";
import { getData, postData } from "@/libs/http";
import { Calendar } from "@/components/forms/Calendar";
import { getLocale } from "@/libs/localization";
import { GeneratedDocuments } from "@/components/appointments/GeneratedDocuments";
import { formatAmount } from "@/libs/formats";
import {alertSuccess} from "@/libs/events/alert";
import {searchContacts} from "@/libs/search";

const Appointments: NextPage = () => {
    const { t } = useTranslation("common");
    const router = useRouter();

    const { data: appointment, mutate: mutateAppointment } = useSWR<Appointment | null>("/data/appointments/" + router.query.mid, loadAppointment);
    const { data: tariffs } = useSWR<Tariff[]>("/data/tariffs/list");

    const {
        register,
        setValue,
        formState: { errors },
        getValues,
        handleSubmit,
    } = useForm();

    const [contactsSuggestions, setContactsSuggestions] = useState<Contact[]>([]);
    const [suggested, setSuggested] = useState<string>();
    const [contact, setContact] = useState<Contact>();
    const [date, setDate] = useState<Date>(startOfHour(new Date()));
    const [duration, setDuration] = useState<number>(60);
    const [customTitle, setCustomTitle] = useState<boolean>(false);

    const paymentOptions = [
        { value: "0", text: t("options.payments.state0") },
        { value: "1", text: t("options.payments.state1") },
        { value: "2", text: t("options.payments.state2") },
        { value: "3", text: t("options.payments.state3") },
    ];

    const stateOptions = [
        { value: "0", text: t("options.appointments.state0") },
        { value: "1", text: t("options.appointments.state1") },
        { value: "10", text: t("options.appointments.state10") },
        { value: "100", text: t("options.appointments.state100") },
    ];

    useEffect(() => {
        if (!appointment) return;

        Object.getOwnPropertyNames(appointment).forEach((p) => {
            setValue(p, (appointment as any)[p]);
        });

        setDuration(appointment.duration ?? 60);

        const d = parseISO(appointment.startDate ?? formatISO(new Date()));
        setDate(d);
        setValue("hour", format(d, "HH:mm"));

        if (appointment.contactId) {
            setContact({
                id: appointment.contactId,
                firstName: appointment.firstName,
                lastName: appointment.lastName,
                birthDate: "",
                phoneNumber: "",
            } as Contact);
        }

        if (appointment.typeId) {
            setValue("tariff", appointment.typeId);
        } else if (appointment.type !== "") {
            setValue("tariff", "-2");
            setCustomTitle(true);
        }
    }, [appointment, tariffs]);

    useEffect(() => {
        if (router.asPath.startsWith("/appointments/new")) {
            let startDate = startOfHour(new Date());

            if (router.query.startDate) {
                startDate = parseISO(router.query.startDate as string);
            }

            setDate(startDate);
            setValue("duration", 60);
            setValue("hour", format(startDate, "HH:mm"));
        }
    }, [router]);

    function loadAppointment(path: string): Promise<Appointment | null> {
        if (path === "/data/appointments/new") {
            return new Promise(() => null);
        }

        return getData(path);
    }

    async function onSubmit(data: any) {
        const tariff = tariffs?.find((x) => x.name == data.type);

        const updatedAppointment: Appointment = {
            id: appointment?.id ?? "",
            contactId: contact?.id ?? null,
            title: data.lastName + " " + data.firstName,
            price: parseFloat(data.price),
            duration: duration,
            startDate: formatISO(date),
            type: data.type,
            typeId: tariff?.id ?? "",
            backgroundColor: tariff?.backgroundColor ?? "#31859c",
            foreColor: tariff?.foreColor ?? "#ffffff",
            state: parseInt(data.state),
            payment: parseInt(data.payment),
            paymentDate: parseInt(data.payment) !== 0 ? formatISO(new Date()) : null,
            firstName: data.firstName,
            lastName: data.lastName,
            birthDate: data.birthDate,
            phoneNumber: data.phoneNumber,
            documents: [],
        };

        if (updatedAppointment.id === "") {
            await postData("/data/appointments/insert", updatedAppointment);
        } else {
            await postData("/data/appointments/update", updatedAppointment);
        }
        await router.back();
        await mutateAppointment();
        alertSuccess(t("alerts.saveSuccess"));
    }

    let searchContactsTimeout: any;

    function startSuggestContacts() {
        if (searchContactsTimeout) {
            clearTimeout(searchContactsTimeout);
        }
        searchContactsTimeout = setTimeout(() => suggestContacts(), 500);
    }

    async function suggestContacts() {
        const lastName = getValues("lastName");
        const firstName = getValues("firstName");

        if (contact && (contact.lastName !== lastName || contact.firstName !== firstName)) {
            setContact(undefined);
        }

        if (lastName === "" && firstName === "") {
            setContactsSuggestions([]);
            return;
        }

        if (lastName + "|" + firstName == suggested) {
            return;
        }

        setContactsSuggestions(await searchContacts({ lastName, firstName }));
        setSuggested(lastName + "|" + firstName);
    }

    const selectContact = useCallback((contact: Contact) => {
        setContact(contact);
        setSuggested(contact.lastName + "|" + contact.firstName);
        setValue("lastName", contact.lastName);
        setValue("firstName", contact.firstName);
        setValue("birthDate", contact.birthDate);
        setValue("phoneNumber", contact.phoneNumber);
        setContactsSuggestions([]);
    }, []);

    function getTariffsOptions() {
        let options =
            tariffs?.map((x) => {
                return {
                    value: x.id,
                    text: x.name + " (" + formatAmount(x.price) + "€)",
                };
            }) ?? [];

        options.unshift({
            value: "-1",
            text: t("pages.appointment.tariffs.empty"),
        });

        options.push({
            value: "-2",
            text: t("pages.appointment.tariffs.other"),
        });

        return options;
    }

    function setAppointmentType(type: string) {
        if (type === "-1") {
            setCustomTitle(false);
            setValue("type", "");
            return;
        }

        if (type === "-2") {
            setCustomTitle(true);
            setValue("type", "");
            computeDate();
            return;
        }

        setCustomTitle(false);
        const tariff = tariffs?.find((x) => x.id == type);

        if (tariff) {
            setValue("type", tariff.name);
            setValue("price", tariff.price?.toFixed(2));
            setValue("duration", tariff.defaultDuration);
        } else {
            setValue("type", "");
            setValue("price", "");
        }
        computeDate();
    }

    function selectDate(d: Date) {
        const newHour = parse(getValues("hour"), "HH:mm", new Date());
        setDate(new Date(d.getFullYear(), d.getMonth(), d.getDate(), newHour.getHours(), newHour.getMinutes()));
    }

    function computeDate() {
        const newDuration = parseInt(getValues("duration"));

        if (newDuration > 0 && newDuration !== duration) {
            setDuration(newDuration);
        }

        const newHour = parse(getValues("hour"), "HH:mm", new Date());

        if (newHour.getFullYear()) {
            if (newHour.getHours() === date.getHours() && newHour.getMinutes() === date.getMinutes()) {
                return;
            }

            setDate(new Date(date.getFullYear(), date.getMonth(), date.getDate(), newHour.getHours(), newHour.getMinutes()));
        }
    }

    function updateState() {
        if (getValues()["payment"] > 0) {
            setValue("state", "10");
        }
    }

    return (
        <ContentContainer>
            <Section>
                <>
                    {appointment?.id === undefined && <h1>{t("pages.appointment.titleNew")}</h1>}
                    {appointment?.id !== undefined && <h1>{t("pages.appointment.titleEditing")}</h1>}
                </>
                <form className={"grid grid-cols-4 gap-4"} onSubmit={handleSubmit(onSubmit)}>
                    <InputText
                        className={"col-span-4 md:col-span-2"}
                        label={t("fields.lastName")}
                        name={"lastName"}
                        autoCapitalize={true}
                        required={true}
                        type={"text"}
                        register={register}
                        setValue={setValue}
                        error={errors.lastName}
                        onChange={() => startSuggestContacts()}
                    />
                    <InputText
                        className={"col-span-4 md:col-span-2"}
                        label={t("fields.firstName")}
                        name={"firstName"}
                        autoCapitalize={true}
                        required={true}
                        type={"text"}
                        register={register}
                        setValue={setValue}
                        error={errors.firstName}
                        onChange={() => startSuggestContacts()}
                    />

                    <InputDate className={"col-span-4 md:col-span-2"} label={t("fields.birthDate")} name={"birthDate"} required={false} type={"text"} register={register} setValue={setValue} error={errors.birthDate} />
                    <InputText
                        className={"col-span-4 md:col-span-2"}
                        label={t("fields.phoneNumber")}
                        name={"phoneNumber"}
                        autoCapitalize={false}
                        required={false}
                        type={"text"}
                        register={register}
                        setValue={setValue}
                        error={errors.phoneNumber}
                    />

                    {contactsSuggestions.length !== 0 && (
                        <div className={"col-span-4"}>
                            <h3>{t("pages.appointment.contactsSuggestions.title")}</h3>
                            {contactsSuggestions.map((p) => (
                                <div key={p.id} className={"cursor-pointer p-2 hover:bg-gray-100"} onClick={() => selectContact(p)}>
                                    {p.lastName} {p.firstName} {p.birthDate && p.birthDate !== "" && <>({p.birthDate})</>}
                                </div>
                            ))}
                        </div>
                    )}

                    <InputSelect className={"col-span-3"} label={t("pages.appointment.tariffs.title")} name={"tariff"} register={register} options={getTariffsOptions()} onChange={(v) => setAppointmentType(v)} />
                    <InputText className={"col-span-1"} label={t("fields.price")} name={"price"} required={true} type={"text"} register={register} setValue={setValue} error={errors.price} />

                    {customTitle && <InputText className={"col-span-4"} label={t("fields.appointmentType")} name={"type"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.type} />}

                    <Calendar className={"col-span-4 md:col-span-2 md:row-span-3"} value={date} onChange={(d) => selectDate(d)} />

                    <InputText className={"col-span-2"} label={t("fields.hour")} name={"hour"} required={true} type={"text"} register={register} setValue={setValue} error={errors.hour} onChange={() => computeDate()} />
                    <InputText className={"col-span-2"} label={t("fields.duration")} name={"duration"} required={true} type={"text"} register={register} setValue={setValue} error={errors.duration} onChange={() => computeDate()} />
                    <div className={"col-span-4 md:col-span-2"}>
                        <div>
                            {format(date, "EEEE dd MMMM", { locale: getLocale() })} {t("fields.fromHour")} {format(date, "HH:mm", { locale: getLocale() })} {t("fields.toHour")} {format(add(date, { minutes: duration }), "HH:mm")}
                        </div>
                    </div>

                    <InputSelect className={"col-span-2"} label={t("fields.payment")} name={"payment"} required={false} register={register} error={errors.payment} options={paymentOptions} onChange={() => updateState()} />
                    <InputSelect className={"col-span-2"} label={t("fields.appointmentState")} name={"state"} required={false} register={register} error={errors.payment} options={stateOptions} />

                    <Button text={t("actions.back")} secondary={true} className={"col-span-2"} onClick={() => router.back()} />

                    <Button text={t("actions.save")} className={"col-span-2"} onClick={handleSubmit(onSubmit)} />
                </form>
            </Section>
            <>{appointment && <GeneratedDocuments appointment={appointment} />}</>
        </ContentContainer>
    );
};

export default Appointments;
