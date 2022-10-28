import styles from '../../styles/components/popups/meeting.module.scss';

import {Popup} from "../Pops";
import useTranslation from "next-translate/useTranslation";
import InputText from "../forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect, useState} from "react";
import {PatientSummary, searchPatients} from "../../lib/services/patients";
import {useMsal} from "@azure/msal-react";
import useSWR from "swr";
import {getTariffs} from "../../lib/services/tariffs";
import {Calendar} from "../forms/Calendar";
import {add, format, formatISO, parse} from "date-fns";
import Button from "../forms/Button";
import {alertSuccess} from "../../lib/events/alert";
import {Meeting, upsertMeeting} from "../../lib/services/meetings";
import InputSelect from "../forms/InputSelect";
import {getLocale} from "../../lib/localization";

interface Props {
    meetingId?: string;
    hide: () => void;
}

export const MeetingPopup = ({meetingId, hide}: Props) => {
    
    const now = new Date();

    const {t} = useTranslation('common');
    const {register, setValue, formState: {errors}, watch, getValues, handleSubmit} = useForm();
    const watchSuggestion = watch(["lastName", "firstName"]);
    const watchHour = watch(["hour", "duration"]);
    const {instance, accounts} = useMsal();
    const [patientsSuggestions, setPatientsSuggestions] = useState<PatientSummary[]>([]);
    const [patient, setPatient] = useState<PatientSummary>();
    const [suggested, setSuggested] = useState<string>();
    const [date, setDate] = useState<Date>(new Date(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), 0, 0));
    const [duration, setDuration] = useState<number>(60);
    
    const paymentOptions = [
        { value: "0", text: t("options.payments.state0") },
        { value: "1", text: t("options.payments.state1") },
        { value: "2", text: t("options.payments.state2") },
        { value: "3", text: t("options.payments.state3") }
    ]

    const stateOptions = [
        { value: "0", text: t("options.meetings.state0") },
        { value: "1", text: t("options.meetings.state1") },
        { value: "10", text: t("options.meetings.state10") },
        { value: "100", text: t("options.meetings.state100") }
    ]

    const loadTariffs = async () => {
        return await getTariffs(instance, accounts[0]);
    }
    const tariffs = useSWR('/tariffs', loadTariffs);

    useEffect(() => {
        setValue("duration", 60);
        setValue("hour", format(date, "HH:mm"));
    }, []);
    
    useEffect(() => {
        const searchPatientsTimeout = setTimeout(() => suggestPatients(), 500);
        return () => clearTimeout(searchPatientsTimeout);
    }, [watchSuggestion]);

    
    useEffect(() => {
        const newDuration = parseInt(getValues("duration"));

        if (newDuration > 0 && newDuration !== duration)
        {
            setDuration(newDuration);
        }
        
        const newHour = parse(getValues('hour'), "HH:mm", new Date());
        
        if (newHour.getFullYear()) {

            if (newHour.getHours() === date.getHours() && newHour.getMinutes() === date.getMinutes())
            {
                return;
            }
            
            setDate(new Date(date.getFullYear(), date.getMonth(), date.getDate(), newHour.getHours(), newHour.getMinutes()));
        }
    }, [watchHour]);

    const suggestPatients = async () => {
        const lastName = getValues('lastName');
        const firstName = getValues('firstName');

        if (lastName === "" && firstName === "") {
            return;
        }

        if (lastName + "|" + firstName == suggested) {
            return;
        }

        const patients = await searchPatients({
            lastName,
            firstName,
            birthDate: '',
            phoneNumber: ''
        }, instance, accounts[0]);
        setPatientsSuggestions(patients);

        setSuggested(lastName + "|" + firstName);
    }

    const selectPatient = (patient: PatientSummary) => {
        setPatient(patient);
        setSuggested(patient.lastName + "|" + patient.firstName);
        setValue("lastName", patient.lastName);
        setValue("firstName", patient.firstName);
        setPatientsSuggestions([]);
    }

    const setMeetingType = (type: string) => {
        const tariff = tariffs.data?.find(x => x.id == type);

        if (tariff) {
            setValue("type", tariff.name);
            setValue("price", tariff.price.toFixed(2));
            setValue("duration", tariff.defaultDuration);
        } else {
            setValue("type", "");
            setValue("price", "");
        }
    }
    
    const onSubmit = async (data: any) => {
        const meeting : Meeting = {
            id: meetingId ?? '',
            patientId: patient?.id ?? null,
            title: data.lastName + " " + data.firstName + (data.type ? " (" + data.type + ")" : ""),
            price: parseFloat(data.price),
            duration: duration,
            startDate: formatISO(date),
            type: data.type,
            state: parseInt(data.state),
            payment: parseInt(data.payment),
            paymentDate: parseInt(data.payment) !== 0 ? formatISO(new Date()) : null 
        }

        await upsertMeeting(meeting, instance, accounts[0]);
        hide();
        alertSuccess(t("alerts.saveSuccess"));
    }

    return <Popup>
        <>
            {meetingId === undefined && <h1>{t("popups.meeting.titleNew")}</h1>}
            {meetingId !== undefined && <h1>{t("popups.meeting.titleEditing")}</h1>}

            <form className={styles.content} onSubmit={handleSubmit(onSubmit)}>
                <InputText className={styles.lastName} label={t("fields.lastName")} name={"lastName"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.lastName}/>
                <InputText className={styles.firstName} label={t("fields.firstName")} name={"firstName"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.firstName}/>
                {patientsSuggestions.length !== 0 && <div className={styles.patientsSuggestions}>
                    <h2>{t("popups.meeting.patientsSuggestions.title")}</h2>
                    {patientsSuggestions.map(p => <div key={p.id} className={styles.patientsSuggestion} onClick={() => selectPatient(p)}>
                        {p.lastName} {p.firstName} {p.birthDate && p.birthDate !== "" && <>({p.birthDate})</>}
                    </div>)}
                </div>}
                <div className={styles.tariffs}>
                    <span>{t("popups.meeting.tariffs.title")}</span>
                    {tariffs.data && <div>
                        <select onChange={(e) => setMeetingType(e.target.value)}>
                            <option value="">{t("popups.meeting.tariffs.empty")}</option>
                            {tariffs.data.map(t => <option key={t.id} value={t.id}>{t.name} ({t.price.toFixed(2)}€)</option>)}
                        </select>
                    </div>
                    }
                </div>
                <InputText className={styles.type} label={t("fields.meetingType")} name={"type"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.type}/>
                <InputText className={styles.price} label={t("fields.price")} name={"price"} required={true} type={"text"} register={register} setValue={setValue} error={errors.price}/>
                <Calendar className={styles.date} value={date} onChange={(d) => setDate(d)}/>
                <InputText className={styles.hour} label={t("fields.hour")} name={"hour"} required={true} type={"text"} register={register} setValue={setValue} error={errors.hour}/>
                <InputText className={styles.duration} label={t("fields.duration")} name={"duration"} required={true} type={"text"} register={register} setValue={setValue} error={errors.duration}/>
                <div className={styles.durationText}>
                    <div>{format(date, "EEEE dd MMMM", { locale: getLocale()})} {t("fields.fromHour")} {format(date, "HH:mm", { locale: getLocale()})} {t("fields.toHour")} {format(add(date, {minutes: duration}), "HH:mm")}</div>
                </div>

                <InputSelect className={styles.payment} label={t("fields.payment")} name={"payment"} required={false} register={register} error={errors.payment} options={paymentOptions} />
                <InputSelect className={styles.state} label={t("fields.meetingState")} name={"state"} required={false} register={register} error={errors.payment} options={stateOptions} />
                
                <Button text={t("actions.cancel")} secondary={true} className={styles.cancel} onClick={() => hide()} />
                <Button text={t("actions.save")}  className={styles.save}  onClick={handleSubmit(onSubmit)}/>
            </form>
        </>
    </Popup>
}