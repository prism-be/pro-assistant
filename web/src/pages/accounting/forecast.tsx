import { ForecastPopup } from "@/components/accounting/ForecastPopup";
import ContentContainer from "@/components/design/ContentContainer";
import { HeaderWithAction } from "@/components/design/HeaderWithAction";
import Section from "@/components/design/Section";
import { ListItem } from "@/components/ListItem";
import { postData } from "@/libs/http";
import { Forecast } from "@/libs/models";
import { NextPage } from "next";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import useSWR from "swr";

const Forecast: NextPage = () => {
    const { t } = useTranslation("accounting");
    const { data: forecasts, mutate: mutateForecasts } = useSWR<Forecast[]>("/data/accounting/forecast");
    const [forecast, setForecast] = useState<Forecast | undefined>(undefined);
    const [editForecast, setEditForecast] = useState(false);

    function createNew(): void {
        setForecast(undefined);
        setEditForecast(true);
    }

    async function create(forecast: Forecast) {
        if (forecast.id === "") {
            await postData("/data/accounting/forecast/insert", forecast);
        } else {
            await postData("/data/accounting/forecast/update", forecast);
        }

        mutateForecasts();
        setEditForecast(false);
    }

    function startEdit(forecast: Forecast) {
        setForecast(forecast);
        setEditForecast(true);
    }

    function deleteForecast(forecast: Forecast) {
        postData("/data/accounting/forecast/delete", forecast).then(() => {
            mutateForecasts();
        });
    }

    return (
        <ContentContainer>
            <Section>
                <>
                    {editForecast && (
                        <ForecastPopup
                            forecast={forecast}
                            onSave={async (data) => {
                                await create(data);
                            }}
                            onCancel={() => setEditForecast(false)}
                        />
                    )}
                </>
                <HeaderWithAction title={t("forecast.title")} action={() => createNew()} actionText={t("common:actions.new")} />
                <div>
                    {forecasts && forecasts.map((item) => <ListItem key={item.id} item={item} title={item.title ?? ""} onEdit={startEdit} onDelete={deleteForecast} />)}
                    {(!forecasts || forecasts.length === 0) && <div className="italic pt-2 text-center">{t("forecast.noForecasts")}</div>}
                </div>
            </Section>
        </ContentContainer>
    );
};

export default Forecast;
