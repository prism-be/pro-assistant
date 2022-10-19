import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import {useMsal} from "@azure/msal-react";
import ContentContainer from "../../components/design/ContentContainer";
import {useRouter} from "next/router";

const Patient: NextPage = () => {
    const {t} = useTranslation('patients');
    const {instance, accounts} = useMsal();
    const router = useRouter();
    const { pid } = router.query;

    return <ContentContainer>
        <>
            <h1>Hello {pid}!</h1>
        </>
    </ContentContainer>
}

export default Patient;