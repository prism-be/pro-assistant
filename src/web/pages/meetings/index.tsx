import type {NextPage} from 'next'
import {useRouter} from "next/router";
import {useEffect} from "react";

const Meeting: NextPage = () => {

    const router = useRouter();

    useEffect(() => {
        router.push('/meetings/new/');
    }, [router]);

    return <></>
}

export default Meeting
