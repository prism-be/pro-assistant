import styles from '../../styles/pages/contacts.module.scss';

import ContentContainer from "../../components/design/ContentContainer";
import InputText from "../../components/forms/InputText";
import InputDate from "../../components/forms/InputDate";
import Button from "../../components/forms/Button";
import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import * as yup from "yup";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {useEffect, useState} from "react";
import {useRouter} from "next/router";
import useKeyboardJs from "react-use/lib/useKeyboardJs";
import {postData} from "../../lib/ajaxHelper";
import {Contact} from "../../lib/contracts";
import Section from "../../components/design/Section";

const Contacts: NextPage = () => {

    const {t} = useTranslation('common');
    const router = useRouter();

    const [contacts, setContacts] = useState<Contact[] | null>(null);

    const schema = yup.object({}).required();

    const {register, handleSubmit, formState: {errors}, setValue, setFocus, reset} = useForm({resolver: yupResolver(schema)});

    useEffect(() => {
        const sessionSearchContacts = sessionStorage.getItem('contacts/search-contacts');
        if (sessionSearchContacts) {
            const data = JSON.parse(sessionSearchContacts);
            Object.getOwnPropertyNames(data).forEach(field => {
                setValue(field, data[field]);
            })
            onSubmit(data);
        }
        setFocus('lastName');
    }, [setFocus])

    const onSubmit = async (data: any) => {
        sessionStorage.setItem('contacts/search-contacts', JSON.stringify(data));
        const result = await postData<Contact[]>("/contacts", data);
        setContacts(result);
    }

    const navigate = async (id: string) => {
        await router.push('/contacts/' + id);
    }

    const resetSearch = () => {
        sessionStorage.removeItem('contacts/search-contacts');
        reset();
        setContacts(null);
        setFocus('lastName');
    }

    const [isNewPressed, isNewPressedEvent] = useKeyboardJs("alt + n")
    useEffect(() => {
        if (isNewPressed) {
            isNewPressedEvent?.preventDefault();
            router.push("/contacts/000000000000000000000000");
        }
    }, [isNewPressed, router])

    return <ContentContainer>
        <>
            <Section>
                <>
                    <h1 className={styles.searchTitle}>{t("pages.contacts.title")}</h1>
                    <form className={styles.searchPanel} onSubmit={handleSubmit(onSubmit)}>
                        <div className={styles.searchField}>
                            <InputText name="lastName" label={t("fields.lastName")} type="text" required={false} register={register} setValue={setValue} error={errors.lastName} autoCapitalize={true}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputText name="firstName" label={t("fields.firstName")} type="text" required={false} register={register} setValue={setValue} error={errors.firstName} autoCapitalize={true}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputText name="phoneNumber" label={t("fields.phoneNumber")} type="text" required={false} register={register} setValue={setValue} error={errors.phoneNumber}/>
                        </div>
                        <div className={styles.searchField}>
                            <InputDate name="birthDate" label={t("fields.birthDate")} type="text" required={false} register={register} setValue={setValue} error={errors.birthDate}/>
                        </div>
                        <div className={styles.resetButton}>
                            <Button text={t("actions.reset")} onClick={resetSearch} secondary={true}/>
                        </div>
                        <div className={styles.newButton}>
                            <Button text={t("actions.new")} onClick={() => router.push("/contacts/000000000000000000000000")} secondary={true}/>
                        </div>
                        <div className={styles.searchButton}>
                            <Button submit={true} text={t("actions.search")} onClick={handleSubmit(onSubmit)}/>
                        </div>
                    </form>
                </>
            </Section>

            <Section>
                <>
                    {contacts && <>
                        <h2>{t("results.title")}</h2>
                        {contacts.length !== 0 && <div className={styles.searchResultsTable}>
                            <div className={styles.searchResultsRow}>
                                <div className={styles.searchResultsHeader}>{t("fields.lastName")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.firstName")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.phoneNumber")}</div>
                                <div className={styles.searchResultsHeader}>{t("fields.birthDate")}</div>
                            </div>
                            {contacts?.map(contact =>
                                <div className={styles.searchResultsRow + " " + styles.searchResultsRowContact} key={contact.id} onClick={() => navigate(contact.id)}>
                                    <div className={styles.searchResultsCell}>{contact.lastName}</div>
                                    <div className={styles.searchResultsCell}>{contact.firstName}</div>
                                    <div className={styles.searchResultsCell}>{contact.phoneNumber}</div>
                                    <div className={styles.searchResultsCell}>{contact.birthDate}</div>
                                </div>
                            )}
                        </div>}

                        {contacts.length === 0 && <div className={styles.helpText}>{t("results.noResults")}</div>}
                    </>}

                    {!contacts && <>
                        <div className={styles.helpText}>{t("results.doSearch")}</div>
                    </>}
                </>
            </Section>
        </>
    </ContentContainer>
}

export default Contacts;