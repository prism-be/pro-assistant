import type { NextPage } from "next";
import ContentContainer from "../components/design/ContentContainer";
import { useRouter } from "next/router";
import { useEffect } from "react";

const Home: NextPage = () => {
    const router = useRouter();

    useEffect(() => {
        router.push("/contacts");
    }, [router]);

    return (
        <ContentContainer>
            <div></div>
        </ContentContainer>
    );
};

export default Home;
