import {NextPage} from "next";
import useTranslation from "next-translate/useTranslation";
import ContentContainer from "../../components/design/ContentContainer";

const Patients: NextPage = () => {

    const {t} = useTranslation('patients');

    return <ContentContainer>
        <h1>{t("title")}</h1>
    </ContentContainer>
}

export default Patients;