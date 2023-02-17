import Section from "../../components/design/Section";
import {NextPage} from "next";
import ContentContainer from "../../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import Lottie from "lottie-react";
import applauseAnimation from "../../animations/lordicon/1092-applause-outline-edited.json";
import useSWR from "swr";
import {Appointment} from "../../lib/contracts";
import {useEffect, useState} from "react";
import {AppointmentsList} from "../../components/appointments/AppointmentsList";
import {useRouter} from "next/router";

const Closing: NextPage = () => {

    const appointments = useSWR("/appointments/opened");
    const [appointmentsByContact, setAppointmentsByContact] = useState<Appointment[][]>([]);
    const router = useRouter();

    useEffect(() => {
        if (appointments.data) {
            const grouped = appointments.data.reduce((group: any, appointment: Appointment) => {

                const {contactId} = appointment;

                if (!contactId) {
                    return group;
                }

                group[contactId] = group[contactId] ?? []
                group[contactId].push(appointment);

                return group;
            }, {});

            const result: Appointment[][] = [];
            let keys = Object.keys(grouped);
            keys.forEach(key => {
                result.push(grouped[key]);
            });

            setAppointmentsByContact(result);
        }
    }, [appointments.data]);

    const {t} = useTranslation("common");

    return <ContentContainer>
        <Section>
            <h1>{t("pages.accounting.closing.title")}</h1>

            <>
                {appointments.data && appointments.data.length === 0 &&
                    <div className={"text-center"}>
                        <div className={"p-4 w-32 m-auto"}>
                            <Lottie animationData={applauseAnimation} loop={false}/>
                        </div>
                        {t("pages.accounting.closing.noUnclosed")}
                    </div>}
            </>

            <>
                {appointmentsByContact.length !== 0 &&
                    <div>
                        {appointmentsByContact.map((appointments) => {
                            return <div className={"pt-4"} key={appointments[0].contactId}>
                                <div>
                                    <div className={"font-bold"}>{appointments[0].lastName} {appointments[0].firstName}</div>
                                </div>
                                <div>
                                    <AppointmentsList appointments={appointments} onClick={(a) => router.push("/appointments/" + a.id)}/>
                                </div>
                            </div>
                        })
                        }
                    </div>
                }
            </>
        </Section>
    </ContentContainer>
}

export default Closing;