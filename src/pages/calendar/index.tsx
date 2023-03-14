import type {NextPage} from 'next'
import {useRouter} from "next/router";
import {useEffect} from "react";
import {format, startOfWeek} from "date-fns";

const Calendar: NextPage = () => {

    const router = useRouter();

    useEffect(() => {
        router.push('/calendar/' + format(startOfWeek(new Date(), {weekStartsOn: 1}), "yyyy-MM-dd"));
    }, [router]);

    return <></>
}

export default Calendar
