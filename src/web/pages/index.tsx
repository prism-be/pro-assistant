import type {NextPage} from 'next'
import Head from 'next/head'
import Image from 'next/image'
import styles from '../styles/Home.module.css'
import useUser from "../lib/useUser";

const Home: NextPage = () => {

    const {user} = useUser({redirectTo: '/login'})

    if (!user || !user.authenticated) {
        return <>In progress</>
    }

    return (
        <div>
            <div className={"p-5"}>
            </div>
        </div>
    )
}

export default Home
