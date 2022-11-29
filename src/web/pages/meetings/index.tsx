import type {NextPage} from 'next'
import {useRouter} from "next/router";
import {useEffect} from "react";
import {format, startOfWeek} from "date-fns";
import {startOfWeekWithOptions} from "date-fns/fp";

const Meeting: NextPage = () => {

    const router = useRouter();

    useEffect(() => {
        router.push('/meetings/new/');
    }, [router]);

    return <></>
}

export default Meeting
