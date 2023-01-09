import styles from '../../styles/pages/contact.module.scss'

import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";
import {useRouter} from "next/router";
import useSWR from "swr";
import InputText from "../../components/forms/InputText";
import {useForm} from "react-hook-form";
import {useEffect} from "react";
import Button from "../../components/forms/Button";
import {alertSuccess} from "../../lib/events/alert";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import {Back} from "../../components/icons/Back";
import InputDate from "../../components/forms/InputDate";
import {getData, postData} from "../../lib/ajaxHelper";
import {Contact, UpsertResult} from "../../lib/contracts";
import Section from "../../components/design/Section";
import {ContactAppointments} from "../../components/contacts/ContactAppointments";

const Contacts: NextPage = () => {
    const {t} = useTranslation('common');
    const router = useRouter();
    const {pid} = router.query;

    const loadContact = async (route: string) => {

        if (route === "/contact/000000000000000000000000") {
            return null;
        }

        return await getData<Contact>(route);
    }

    const {data: contact, mutate: mutateContact} = useSWR("/contact/" + pid, loadContact);
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();

    useEffect(() => {
        if (contact) {
            const d: any = contact;
            Object.getOwnPropertyNames(contact).forEach(field => {
                setValue(field, d[field]);
            })
        }
    }, [contact, setValue])


    const saveContactForm = async (data: any) => {
        if (pid === '000000000000000000000000') {
            data.id = '';
            const newPid = await postData<UpsertResult>("/contact", data);
            alertSuccess(t("details.saveSuccess"), {autoClose: true});
            await router.push("/contacts/" + newPid?.id);
            return;
        }

        await postData("/contact", data);
        await mutateContact();
        alertSuccess(t("pages.contacts.details.saveSuccess"), {autoClose: true});
    }

    const onSaveContactSubmit = async (data: any) => {
        await saveContactForm(data);
    }

    const [isSavePressed, isSaveEvent] = useKeyboardJs("ctrl + s")
    useEffect(() => {
        if (isSavePressed) {
            isSaveEvent?.preventDefault();
            const data = getValues();
            saveContactForm(data);
        }
    }, [isSavePressed, getValues])


    return <ContentContainer>
        <Section>
            <div className={styles.cardTitle}>
                <a onClick={() => router.push("/contacts")}>
                    <Back/>
                </a>
                <h1>{t("pages.contacts.details.title")} {contact?.lastName} {contact?.firstName}</h1>
            </div>
            <form className={styles.contact} onSubmit={handleSubmit(onSaveContactSubmit)}>
                <div className={styles.contactFieldExtraSmall}>
                    <InputText name="title" label={t("fields.title")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldAverageSmall}>
                    <InputText name="lastName" label={t("fields.lastName")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldAverageSmall}>
                    <InputText name="firstName" label={t("fields.firstName")} type="text" required={false} register={register} setValue={setValue} error={errors.firstName} autoCapitalize={true}/>
                </div>
                <div className={styles.contactField}>
                    <InputText name="email" label={t("fields.email")} type="text" required={false} register={register} setValue={setValue} error={errors.email}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="phoneNumber" label={t("fields.phoneNumber")} type="text" required={false} register={register} setValue={setValue} error={errors.phoneNumber}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputDate name="birthDate" label={t("fields.birthDate")} type="text" required={false} register={register} setValue={setValue} error={errors.birthDate}/>
                </div>
                <div className={styles.contactFieldWide}>
                    <InputText name="street" label={t("fields.street")} type="text" required={false} register={register} setValue={setValue} error={errors.street} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="number" label={t("fields.number")} type="text" required={false} register={register} setValue={setValue} error={errors.number}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="zipCode" label={t("fields.zipCode")} type="text" required={false} register={register} setValue={setValue} error={errors.zipCode}/>
                </div>
                <div className={styles.contactField}>
                    <InputText name="city" label={t("fields.city")} type="text" required={false} register={register} setValue={setValue} error={errors.city} autoCapitalize={true}/>
                </div>
                <div className={styles.contactFieldSmall}>
                    <InputText name="country" label={t("fields.country")} type="text" required={false} register={register} setValue={setValue} error={errors.country} autoCapitalize={true}/>
                </div>
                <div className={styles.saveButton}>
                    <Button text={t("actions.save")} onClick={handleSubmit(onSaveContactSubmit)} secondary={true}/>
                </div>
            </form>
        </Section>
        <ContactAppointments contactId={pid as string}/>
    </ContentContainer>
}

export default Contacts;