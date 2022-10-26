import styles from '../../styles/components/popups/meeting.module.scss';

import {Popup} from "../Pops";
import useTranslation from "next-translate/useTranslation";
import InputText from "../forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect, useState} from "react";
import {PatientSummary, searchPatients} from "../../lib/services/patients";
import {useMsal} from "@azure/msal-react";

interface Props {
    meetingId?: string;
}

export const MeetingPopup = ({meetingId}: Props) => {
    const {t} = useTranslation('common');
    const {register, setValue, formState: {errors}, watch, getValues} = useForm();
    const watchFields = watch(["lastName", "firstName"]);
    const {instance, accounts} = useMsal();
    const [patientsSuggestions, setPatientsSuggestions]= useState<PatientSummary[]>([]);
    const [patient, setPatient]= useState<PatientSummary>();
    const [suggested, setSuggested]= useState<string>();

    useEffect(() => {
        const searchPatientsTimeout = setTimeout(() => suggestPatients(), 500);
        return () => clearTimeout(searchPatientsTimeout);
    }, [watchFields]);

    const suggestPatients = async () => {
        const lastName = getValues('lastName');
        const firstName = getValues('firstName');
        
        if (lastName === "" && firstName === "")
        {
            return;
        }
        
        console.log(suggested);
        
        if (lastName + "|" + firstName == suggested)
        {
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
    
    return <Popup>
        <>
            {meetingId === undefined && <h1>{t("popups.meeting.titleNew")}</h1>}
            {meetingId !== undefined && <h1>{t("popups.meeting.titleEditing")}</h1>}
            
            <form className={styles.content}>
                <InputText className={styles.lastName} label={t("fields.lastName")} name={"lastName"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.lastName} />
                <InputText className={styles.firstName} label={t("fields.firstName")} name={"firstName"} autoCapitalize={true} required={false} type={"text"} register={register} setValue={setValue} error={errors.firstName} />
                {patientsSuggestions.length !== 0 && <div className={styles.patientsSuggestions}>
                    <h2>{t("popups.meeting.patientsSuggestions.title")}</h2>
                    {patientsSuggestions.map(p => <div key={p.id} className={styles.patientsSuggestion} onClick={() => selectPatient(p)}>
                        {p.lastName} {p.firstName} {p.birthDate && p.birthDate !== "" && <>({p.birthDate})</>}
                    </div>)}
                </div>}
            </form>
        </>
    </Popup>
}