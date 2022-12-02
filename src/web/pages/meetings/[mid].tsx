import styles from '../../styles/pages/meeting.module.scss';
import {NextPage} from "next";
import ContentContainer from "../../components/design/ContentContainer";
import Section from "../../components/design/Section";
import useTranslation from "next-translate/useTranslation";
import {useRouter} from "next/router";
import useSWR from "swr";
import {useForm} from "react-hook-form";
import {useEffect, useState} from "react";
import InputText from "../../components/forms/InputText";
import {IDocumentConfiguration, IMeeting, IPatientSummary, ITariff} from "../../lib/contracts";
import {downloadDocument, getData, postData} from "../../lib/ajaxHelper";
import InputSelect from "../../components/forms/InputSelect";
import {Calendar} from "../../components/forms/Calendar";
import {add, format, formatISO, parse, parseISO, startOfHour} from "date-fns";
import {getLocale} from "../../lib/localization";
import Button from "../../components/forms/Button";
import {Save} from "../../components/icons/Save";
import {alertSuccess} from "../../lib/events/alert";

const Meeting: NextPage = () => {
    const {t} = useTranslation('common');
    const router = useRouter();
    
    const {data: meeting, mutate: mutateMeeting} = useSWR<IMeeting | null>("/meeting/" + router.query.mid, loadMeeting);
    const {data: tariffs} = useSWR<ITariff[]>('/tariffs');
    const {data: documents} = useSWR<IDocumentConfiguration[]>("/documents-configuration");
    
    const {register, setValue, formState: {errors}, getValues, handleSubmit} = useForm();
    
    const [patientsSuggestions, setPatientsSuggestions] = useState<IPatientSummary[]>([]);
    const [suggested, setSuggested] = useState<string>();
    const [patient, setPatient] = useState<IPatientSummary>();
    const [date, setDate] = useState<Date>(startOfHour(new Date()));
    const [duration, setDuration] = useState<number>(60);
    const [document, setDocument] = useState<string>();

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
    
    useEffect(() => {
        if (!meeting) return;
        
        Object.getOwnPropertyNames(meeting).forEach(p => {
            setValue(p, (meeting as any)[p]);
        });

        setDuration(meeting.duration);
        
        const d = parseISO(meeting.startDate);
        setDate(d);
        setValue("hour", format(d, "HH:mm"));

        if (meeting.typeId) {
            setValue("tariff", meeting.typeId);
        }
        
    }, [meeting, tariffs]);
    
    useEffect(() => {
        if (router.asPath.startsWith("/meetings/new"))
        {
            let startDate = startOfHour(new Date());

            if (router.query.startDate)
            {
                startDate = parseISO(router.query.startDate as string);
            }

            setDate(startDate);
            setValue("duration", 60);
            setValue("hour", format(startDate, "HH:mm"));
        }
    }, [router])
    
    function loadMeeting(path: string): Promise<IMeeting | null>
    {
        if (path === "/meeting/new")
        {
            return new Promise(() => null);
        }
        
        return getData(path);
    }
    
    async function onSubmit(data: any)
    {
        const tariff = tariffs?.find(x => x.name == data.type);
        
        const updatedMeeting: IMeeting = {
            id: meeting?.id ?? '',
            patientId: patient?.id ?? null,
            title: data.lastName + " " + data.firstName + (data.type ? " (" + data.type + ")" : ""),
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
            lastName: data.lastName
        }

        await postData("/meeting", updatedMeeting);
        await router.back();
        await mutateMeeting();
        alertSuccess(t("alerts.saveSuccess"));
    }

    let searchPatientsTimeout: any;
    function startSuggestPatients()
    {
        if (searchPatientsTimeout) {
            clearTimeout(searchPatientsTimeout);
        }
        searchPatientsTimeout = setTimeout(() => suggestPatients(), 500);
    }
    
    async function suggestPatients() {
        const lastName = getValues('lastName');
        const firstName = getValues('firstName');

        if (lastName === "" && firstName === "") {
            setPatientsSuggestions([]);
            return;
        }

        if (lastName + "|" + firstName == suggested) {
            return;
        }

        const patients = await postData<IPatientSummary[]>("/patients", {
            lastName,
            firstName,
            birthDate: '',
            phoneNumber: ''
        });
        
        setPatientsSuggestions(patients ?? []);
        setSuggested(lastName + "|" + firstName);
    }
    
    function selectPatient(patient: IPatientSummary)
    {
        setPatient(patient);
        setSuggested(patient.lastName + "|" + patient.firstName);
        setValue("lastName", patient.lastName);
        setValue("firstName", patient.firstName);
        setPatientsSuggestions([]);
    }

    function getTariffsOptions() {
        let options = tariffs?.map(x => {
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

    function setMeetingType(type: string) {
        const tariff = tariffs?.find(x => x.id == type);

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

    function selectDate(d: Date) {
        const newHour = parse(getValues('hour'), "HH:mm", new Date());
        setDate(new Date(d.getFullYear(), d.getMonth(), d.getDate(), newHour.getHours(), newHour.getMinutes()));
    }

    function computeDate() {
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

    async function startDownloadDocument() {
        if (!document || document === "" || meeting?.id == null) {
            return;
        }

        await downloadDocument(document, meeting.id);
    }

    return <ContentContainer>
        <Section>
            <>
                {meeting?.id === undefined && <h1>{t("popups.meeting.titleNew")}</h1>}
                {meeting?.id !== undefined && <h1>{t("popups.meeting.titleEditing")}</h1>}
            </>
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

                <Button text={t("actions.back")} secondary={true} className={styles.cancel} onClick={() => router.back()}/>

                <div className={styles.documents}>
                    <select onChange={(e) => setDocument(e.target.value)}>
                        <option value={""}>{t("popups.meeting.generateDocument")}</option>
                        {documents?.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
                    </select>
                    <div onClick={() => startDownloadDocument()} className={styles.documentsSave}>
                        <Save />
                    </div>
                </div>

                <Button text={t("actions.save")} className={styles.save} onClick={handleSubmit(onSubmit)}/>
            </form>
        </Section>
    </ContentContainer>
}

export default Meeting;