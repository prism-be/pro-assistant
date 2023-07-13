import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import useSWR from "swr";

const Reporting: NextPage = () => {
    const {t} = useTranslation("accounting");

    const periods = useSWR("/data/accounting/reporting/periods");
    
    return <ContentContainer>
        <Section>
            <h1>{t("reporting.title")}</h1>
            <>
            </>
        </Section>
    </ContentContainer>;
}

export default Reporting;