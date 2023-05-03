import useSWR from "swr";
import Section from "../design/Section";
import {useTranslation} from "react-i18next";
import {useRouter} from "next/router";
import {AppointmentsList} from "../appointments/AppointmentsList";
import {Appointment, SearchFilter} from "@/libs/models";
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
        }] as SearchFilter[]);
        data?.reverse();
        return data ?? [];
    }

    const {data: appointments} = useSWR<Appointment[]>(
        "/api/contacts/" + props.contactId + "/appointments",
        loadAppointments
    );

    const {t} = useTranslation("common");
    const router = useRouter();

    async function displayAppointment(id: string | null) {
        await router.push("/appointments/" + id);
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
