import useSWR from "swr";
import Section from "../design/Section";
import {useTranslation} from "react-i18next";
import {AppointmentsList} from "@/components/appointments/AppointmentsList";
import {Appointment, Filter} from "@/libs/models";
import {postData} from "@/libs/http";

export interface Props {
    contactId: string;
}

export const ContactAppointments = (props: Props) => {
    async function loadAppointments(): Promise<Appointment[]> {

        if (props.contactId === "000000000000000000000000") return Promise.resolve([]);

        const data = await postData<Appointment[]>("/data/appointments/search", [{
            field: "ContactId",
            operator: "eq",
            value: props.contactId,
        }] as Filter[]);
        data?.reverse();
        return data ?? [];
    }

    const {data: appointments} = useSWR<Appointment[]>(
        "/api/contacts/" + props.contactId + "/appointments",
        loadAppointments
    );

    const {t} = useTranslation("common");

    async function displayAppointment(id: string | null) {
        window.location.assign("/appointments/" + id);
    }

    return (
        <Section>
            <h2>{t("pages.contacts.details.appointments.title")}</h2>
            <div>
                <AppointmentsList appointments={appointments ?? []} onClick={(m) => displayAppointment(m.id)}/>
            </div>
            <>
                {appointments?.length === 0 && (
                    <div className={"componentStyles.noAppointments"}>
                        {t("pages.contacts.details.appointments.noAppointments")}
                    </div>
                )}
            </>
        </Section>
    );
};
