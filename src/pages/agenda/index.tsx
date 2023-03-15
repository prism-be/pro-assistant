import type { NextPage } from "next";
import { useRouter } from "next/router";
import { useEffect } from "react";
import { format } from "date-fns";

const Agenda: NextPage = () => {
    const router = useRouter();

    useEffect(() => {
        router.push("/agenda/" + format(new Date(), "yyyy-MM-dd"));
    }, [router]);

    return <></>;
};

export default Agenda;
