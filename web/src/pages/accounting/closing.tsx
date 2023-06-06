import {NextPage} from "next";
import Section from "@/components/design/Section";
import ContentContainer from "@/components/design/ContentContainer";
import {useTranslation} from "react-i18next";
import {Appointment, AppointmentClosing} from "@/libs/models";
import useSWR from "swr";
import {AppointmentsList} from "@/components/appointments/AppointmentsList";
import {useState} from "react";
import {Popup} from "@/components/Pops";
import Button from "@/components/forms/Button";
import sortArray from "sort-array";
import InputSelect from "@/components/forms/InputSelect";
import {useForm} from "react-hook-form";
import {postData} from "@/libs/http";
import {useRouter} from "next/router";
import InputDate from "@/components/forms/InputDate";
import {format, formatISO, parse, parseISO} from "date-fns";
import {HandThumbUpIcon} from "@heroicons/react/24/solid";

const Closing: NextPage = () => {

    const router = useRouter();
    const {t} = useTranslation("accounting");
    const {data: unclosed, mutate: mutateUnclosed} = useSWR<Appointment[]>("/data/accounting/closing/unclosed");

    const [currentEdit, setCurrentEdit] = useState<Appointment | null>(null);

    const paymentOptions = [
        {value: "0", text: t("common:options.payments.state0")},
        {value: "1", text: t("common:options.payments.state1")},
        {value: "2", text: t("common:options.payments.state2")},
        {value: "3", text: t("common:options.payments.state3")},
    ];

    const stateOptions = [
        {value: "0", text: t("common:options.appointments.state0")},
        {value: "1", text: t("common:options.appointments.state1")},
        {value: "10", text: t("common:options.appointments.state10")},
        {value: "100", text: t("common:options.appointments.state100")},
    ];

    const {
        register,
        setValue,
        formState: {errors},
        getValues,
        handleSubmit,
    } = useForm();

    function updateState() {
        if (getValues()["payment"] > 0) {
            setValue("state", "10");
            if (currentEdit?.startDate) {
                setValue("paymentDate", format(parseISO(currentEdit.startDate), "dd/MM/yyyy"));
            } else {
                setValue("paymentDate", format(new Date(), "dd/MM/yyyy"));
            }
        }
    }

    function startEditing(appointment: Appointment) {
        setCurrentEdit(appointment);
        setValue("payment", appointment.payment);
        setValue("state", appointment.state);

        if (appointment.paymentDate && appointment.paymentDate !== "") {
            setValue("paymentDate", format(parseISO(appointment.startDate), "dd/MM/yyyy"));
        }
    }

    async function onSubmit() {

        if (!currentEdit)
            return;

        let appointmentClosing: AppointmentClosing = {
            id: currentEdit.id,
            payment: getValues()["payment"],
            state: getValues()["state"],
        };

        if (getValues()["paymentDate"] && getValues()["paymentDate"] !== "") {
            appointmentClosing.paymentDate = formatISO(parse(getValues()["paymentDate"], "dd/MM/yyyy", new Date()));
        }

        await postData("/data/appointments/close", appointmentClosing);
        setCurrentEdit(null);
        await mutateUnclosed();
    }

    return <ContentContainer>
        <Section>
            <h1>{t("closing.title")}</h1>
            <>
                {currentEdit && <Popup>
                    <h2>Modifier {currentEdit.title}</h2>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <InputSelect className={"mb-3"} label={t("common:fields.payment")} name={"payment"}
                                     required={false} register={register} error={errors.payment}
                                     options={paymentOptions} onChange={() => updateState()}/>

                        <InputDate className={"mb-3"} label={t("common:fields.paymentDate")} name={"paymentDate"}
                                   required={false} type={"text"} register={register} setValue={setValue}
                                   error={errors.paymentDate}/>

                        <InputSelect className={"mb-3"} label={t("common:fields.appointmentState")}
                                     name={"state"} required={false} register={register} error={errors.payment}
                                     options={stateOptions}/>

                        <div className={"grid grid-cols-3 gap-2"}>
                            <Button
                                text={t("common:actions.cancel")}
                                onClick={() => setCurrentEdit(null)}
                                secondary={true}
                            />
                            <Button
                                text={t("common:actions.details")}
                                onClick={() => router.push(`/appointments/${currentEdit.id}`)}
                                secondary={true}
                            />
                            <Button text={t("common:actions.save")} onClick={handleSubmit(onSubmit)}/>
                        </div>
                    </form>
                </Popup>}
            </>

            <>{unclosed?.length === 0 && <div className={"p-2 text-green-700 text-center"}>
                <HandThumbUpIcon className={"inline-block w-10 mr-2"}/>
                <p>
                    {t("closing.noUnclosed")}
                </p>
            </div>}
            </>

            <>
                <AppointmentsList appointments={sortArray(unclosed ?? [], {by: "startDate"})}
                                  onClick={(x) => startEditing(x)}/>
            </>

        </Section>
    </ContentContainer>
}

export default Closing;