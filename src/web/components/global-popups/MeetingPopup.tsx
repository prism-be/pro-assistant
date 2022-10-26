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

interface Props {
    meetingId?: string;
}

export const MeetingPopup = ({meetingId}: Props) => {
    const {t} = useTranslation('common');
    const {register, setValue, formState: {errors}, watch, getValues} = useForm();
    const watchFields = watch(["lastName", "firstName"]);
    const {instance, accounts} = useMsal();
    const [patientsSuggestions, setPatientsSuggestions] = useState<PatientSummary[]>([]);
    const [patient, setPatient] = useState<PatientSummary>();
    const [suggested, setSuggested] = useState<string>();

    const loadTariffs = async () => {
        return await getTariffs(instance, accounts[0]);
    }
    const tariffs = useSWR('/tariffs', loadTariffs);

    useEffect(() => {
        const searchPatientsTimeout = setTimeout(() => suggestPatients(), 500);
        return () => clearTimeout(searchPatientsTimeout);
    }, [watchFields]);

    const suggestPatients = async () => {
        const lastName = getValues('lastName');
        const firstName = getValues('firstName');

        if (lastName === "" && firstName === "") {
            return;
        }

        console.log(suggested);

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
        }
        else
        {
            setValue("type", "");
            setValue("price", "");
        }
    }

    return <Popup>
        <>
            {meetingId === undefined && <h1>{t("popups.meeting.titleNew")}</h1>}
            {meetingId !== undefined && <h1>{t("popups.meeting.titleEditing")}</h1>}

            <form className={styles.content}>
                <InputText className={styles.lastName} label={t("fields.lastName")} name={"lastName"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.lastName}/>
                <InputText className={styles.firstName} label={t("fields.firstName")} name={"firstName"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.firstName}/>
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
                <InputText className={styles.type} label={t("fields.meetingType")} name={"type"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.type}/>
                <InputText className={styles.price} label={t("fields.price")} name={"price"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.price}/>

            </form>
        </>
    </Popup>
}