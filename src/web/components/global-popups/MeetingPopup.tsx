import styles from '../../styles/components/popups/meeting.module.scss';

import {Popup} from "../Pops";
import useTranslation from "next-translate/useTranslation";
import InputText from "../forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect, useState} from "react";
import useSWR from "swr";
import {Calendar} from "../forms/Calendar";
import {add, format, formatISO, parse, parseISO} from "date-fns";
import Button from "../forms/Button";
import {alertSuccess} from "../../lib/events/alert";
import InputSelect from "../forms/InputSelect";
import {getLocale} from "../../lib/localization";
import {dataUpdated} from "../../lib/events/data";
import {downloadDocument, getData, postData} from "../../lib/ajaxHelper";
import {IDocument, IMeeting, IPatient, IPatientSummary, ITariff} from "../../lib/contracts";

interface Props {
    data?: any;
    hide: () => void;
}

export const MeetingPopup = ({data, hide}: Props) => {

    const now = new Date();

    const {t} = useTranslation('common');
    const {register, setValue, formState: {errors}, getValues, handleSubmit} = useForm();
    const [patientsSuggestions, setPatientsSuggestions] = useState<IPatientSummary[]>([]);
    const [patient, setPatient] = useState<IPatientSummary>();
    const [suggested, setSuggested] = useState<string>();
    const [date, setDate] = useState<Date>(data?.startDate ?? new Date(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), 0, 0));
    const [duration, setDuration] = useState<number>(60);
    const [meetingId, setMeetingId] = useState<string>();
    const {data: documents} = useSWR<IDocument[]>("/documents");

    const paymentOptions = [
        {value: "0", text: t("options.payments.state0")},
        {value: "1", text: t("options.payments.state1")},
        {value: "2", text: t("options.payments.state2")},
        {value: "3", text: t("options.payments.state3")}
    ]

    const stateOptions = [
        {value: "0", text: t("options.meetings.state0")},
        {value: "1", text: t("options.meetings.state1")},
        {value: "10", text: t("options.meetings.state10")},
        {value: "100", text: t("options.meetings.state100")}
    ]

    const tariffs = useSWR<ITariff[]>('/tariffs');

    useEffect(() => {
        if (data?.meetingId) {
            setMeetingId(data?.meetingId);
            loadExistingMeeting(data?.meetingId);
            return;
        }

        setValue("duration", 60);
        setValue("hour", format(date, "HH:mm"));

    }, [data]);

    const loadExistingMeeting = async (meetingId: string) => {
        const m = await getData<IMeeting>("/meeting/" + meetingId);
        Object.getOwnPropertyNames(m).forEach(p => {
            setValue(p, (m as any)[p]);
        });
        
        if (m == null)
        {
            return;
        }

        if (m.patientId) {
            const patient = await getData<IPatient>("/patient/" + m.patientId);

            if (patient) {
                setPatient(patient);
            }
        }

        const d = parseISO(m.startDate);
        setDate(d);
        setDuration(m.duration);
        setValue("hour", format(d, "HH:mm"));

        const tariff = tariffs.data?.find(x => x.name == m.type);

        if (tariff) {
            setValue("tariff", tariff.id);
        }
    }

    const computeDate = () => {
        const newDuration = parseInt(getValues("duration"));

        if (newDuration > 0 && newDuration !== duration) {
            setDuration(newDuration);
        }

        const newHour = parse(getValues('hour'), "HH:mm", new Date());

        if (newHour.getFullYear()) {

            if (newHour.getHours() === date.getHours() && newHour.getMinutes() === date.getMinutes()) {
                return;
            }

            setDate(new Date(date.getFullYear(), date.getMonth(), date.getDate(), newHour.getHours(), newHour.getMinutes()));
        }
    }

    const selectDate = (d: Date) => {
        const newHour = parse(getValues('hour'), "HH:mm", new Date());
        setDate(new Date(d.getFullYear(), d.getMonth(), d.getDate(), newHour.getHours(), newHour.getMinutes()));
    }

    let searchPatientsTimeout: any;
    const startSuggestPatients = () => {
        if (searchPatientsTimeout) {
            clearTimeout(searchPatientsTimeout);
        }
        searchPatientsTimeout = setTimeout(() => suggestPatients(), 500);
    }

    const suggestPatients = async () => {
        const lastName = getValues('lastName');
        const firstName = getValues('firstName');

        if (lastName === "" && firstName === "") {
            setPatientsSuggestions([]);
            return;
        }

        if (lastName + "|" + firstName == suggested) {
            return;
        }

        const patients = await postData<IPatientSummary[]>("/patients",{
            lastName,
            firstName,
            birthDate: '',
            phoneNumber: ''
        });
        setPatientsSuggestions(patients ?? []);

        setSuggested(lastName + "|" + firstName);
    }

    const selectPatient = (patient: IPatientSummary) => {
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
        computeDate();
    }

    const onSubmit = async (data: any) => {
        const meeting: IMeeting = {
            id: meetingId ?? '',
            patientId: patient?.id ?? null,
            title: data.lastName + " " + data.firstName + (data.type ? " (" + data.type + ")" : ""),
            price: parseFloat(data.price),
            duration: duration,
            startDate: formatISO(date),
            type: data.type,
            state: parseInt(data.state),
            payment: parseInt(data.payment),
            paymentDate: parseInt(data.payment) !== 0 ? formatISO(new Date()) : null,
            firstName: data.firstName,
            lastName: data.lastName
        }

        await postData("/meeting", meeting);
        hide();
        alertSuccess(t("alerts.saveSuccess"));
        dataUpdated({type: "meeting"});
    }

    const getTariffsOptions = () => {
        let options = tariffs.data?.map(x => {
            return {
                value: x.id,
                text: x.name + " (" + x.price.toFixed(2) + "€)"
            }
        }) ?? [];

        options.unshift({
            value: "",
            text: t("popups.meeting.tariffs.empty")
        });

        return options;
    }
    
    const generateDocument = async (documentId: string) => {
        if (documentId === "" || meetingId == null)
        {
            return;
        }
        
        await downloadDocument(documentId, meetingId);
    }

    return <Popup>
        <>
            {data?.meetingId === undefined && <h1>{t("popups.meeting.titleNew")}</h1>}
            {data?.meetingId !== undefined && <h1>{t("popups.meeting.titleEditing")}</h1>}

            <form className={styles.content} onSubmit={handleSubmit(onSubmit)}>
                <InputText className={styles.lastName} label={t("fields.lastName")} name={"lastName"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.lastName} onChange={() => startSuggestPatients()}/>
                <InputText className={styles.firstName} label={t("fields.firstName")} name={"firstName"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.firstName} onChange={() => startSuggestPatients()}/>
                {patientsSuggestions.length !== 0 && <div className={styles.patientsSuggestions}>
                    <h2>{t("popups.meeting.patientsSuggestions.title")}</h2>
                    {patientsSuggestions.map(p => <div key={p.id} className={styles.patientsSuggestion} onClick={() => selectPatient(p)}>
                        {p.lastName} {p.firstName} {p.birthDate && p.birthDate !== "" && <>({p.birthDate})</>}
                    </div>)}
                </div>}
                <InputSelect className={styles.tariffs} label={t("popups.meeting.tariffs.title")} name={"tariff"} register={register} options={getTariffsOptions()} onChange={(v) => setMeetingType(v)}/>
                <InputText className={styles.type} label={t("fields.meetingType")} name={"type"} autoCapitalize={true} required={true} type={"text"} register={register} setValue={setValue} error={errors.type}/>
                <InputText className={styles.price} label={t("fields.price")} name={"price"} required={true} type={"text"} register={register} setValue={setValue} error={errors.price}/>
                <Calendar className={styles.date} value={date} onChange={(d) => selectDate(d)}/>
                <InputText className={styles.hour} label={t("fields.hour")} name={"hour"} required={true} type={"text"} register={register} setValue={setValue} error={errors.hour} onChange={() => computeDate()}/>
                <InputText className={styles.duration} label={t("fields.duration")} name={"duration"} required={true} type={"text"} register={register} setValue={setValue} error={errors.duration} onChange={() => computeDate()}/>
                <div className={styles.durationText}>
                    <div>{format(date, "EEEE dd MMMM", {locale: getLocale()})} {t("fields.fromHour")} {format(date, "HH:mm", {locale: getLocale()})} {t("fields.toHour")} {format(add(date, {minutes: duration}), "HH:mm")}</div>
                </div>

                <InputSelect className={styles.payment} label={t("fields.payment")} name={"payment"} required={false} register={register} error={errors.payment} options={paymentOptions}/>
                <InputSelect className={styles.state} label={t("fields.meetingState")} name={"state"} required={false} register={register} error={errors.payment} options={stateOptions}/>

                <Button text={t("actions.cancel")} secondary={true} className={styles.cancel} onClick={() => hide()}/>
                
                <select className={styles.documents} onChange={(e) => generateDocument(e.target.value)}>
                    <option value={""}>{t("popups.meeting.generateDocument")}</option>
                    { documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>) }
                </select>
                
                <Button text={t("actions.save")} className={styles.save} onClick={handleSubmit(onSubmit)}/>
            </form>
        </>
    </Popup>
}