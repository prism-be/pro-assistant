import ContentContainer from "@/components/design/ContentContainer";
import { HeaderWithAction } from "@/components/design/HeaderWithAction";
import Section from "@/components/design/Section";
import { ListItem } from "@/components/ListItem";
import { Forecast } from "@/libs/models";
import { NextPage } from "next";
import { useTranslation } from "react-i18next";
import useSWR from "swr";

const Forecast: NextPage = () => {
    const { t } = useTranslation("common");

    const { data: forecasts, mutate: mutateForecasts } = useSWR<Forecast[]>("/data/accounting/forecast");

    function createNew(): void {
        throw new Error("Function not implemented.");
    }

    return <ContentContainer>
        <Section>
            <HeaderWithAction title={t("pages.accounting.forecast.title")} action={() => createNew()} actionText={t("common:actions.new")}
            />
            <div>
                {forecasts && forecasts.map((item) => <ListItem key={item.id} item={item} title={item.title ?? ''} />)}
                {(!forecasts || forecasts.length === 0) && <div className="italic">{t("pages.accounting.forecast.noForecasts")}</div>}
            </div>
        </Section>
    </ContentContainer>
}

export default Forecast;