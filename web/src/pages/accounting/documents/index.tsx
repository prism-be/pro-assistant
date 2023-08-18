import type {NextPage} from "next";
import {useRouter} from "next/router";
import {useEffect} from "react";

const Appointment: NextPage = () => {
    const router = useRouter();

    useEffect(() => {
        router.push("/accounting/documents/" + new Date().getFullYear());
    }, [router]);

    return <></>;
};

export default Appointment;
