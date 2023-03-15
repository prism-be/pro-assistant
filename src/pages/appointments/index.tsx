import type { NextPage } from "next";
import { useRouter } from "next/router";
import { useEffect } from "react";

const Appointment: NextPage = () => {
    const router = useRouter();

    useEffect(() => {
        router.push("/appointments/new/");
    }, [router]);

    return <></>;
};

export default Appointment;
