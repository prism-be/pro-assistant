import Section from "../../components/design/Section";
import {NextPage} from "next";
import ContentContainer from "../../components/design/ContentContainer";
import useTranslation from "next-translate/useTranslation";
import {HandThumbUpIcon} from "@heroicons/react/24/solid";
import Image from "next/image";
import Lottie from "lottie-react";
import applauseAnimation from "../../animations/lordicon/1092-applause-outline-edited.json";

const Closing: NextPage = () => {

    const {t} = useTranslation("common");

    return <ContentContainer>
        <Section>
            <h1>{t("pages.accounting.closing.title")}</h1>
            <div className={"text-center"}>
                <div className={"p-4 w-32 m-auto"}>
                    <Lottie animationData={applauseAnimation} loop={false}/>
                </div>
                {t("pages.accounting.closing.noUnclosed")}
            </div>
        </Section>
    </ContentContainer>
}

export default Closing;