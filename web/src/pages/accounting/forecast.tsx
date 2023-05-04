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

    const [editingForecast, setEditingForecast] = useState<Forecast | undefined>(undefined);
    const [editForecast, setEditForecast] = useState(false);
    const [forecast, setForecast] = useState<Forecast | undefined>(undefined);

    function createNew(): void {
        setEditingForecast(undefined);
        setEditForecast(true);
    }

    function create(forecast: Forecast) {
        const p = forecast.id === "" ? postData("/data/accounting/forecast/insert", forecast) : postData("/data/accounting/forecast/update", forecast);

        p.then(() => {
            mutateForecasts();
            setEditForecast(false);
        }).catch((error) => {
            console.log(error);
        });
    }

    function startEdit(forecast: Forecast) {
        setEditingForecast(forecast);
        setEditForecast(true);
    }

    function deleteForecast(forecast: Forecast) {
        postData("/data/accounting/forecast/delete", forecast)
            .then(() => {
                mutateForecasts();
            })
            .catch((error) => {
                console.log(error);
            });
    }

    function selectForecast(forecast: Forecast) {
        setForecast(forecast);
    }

    return (
        <ContentContainer>
            <Section>
                <>{editForecast && <ForecastPopup forecast={editingForecast} onSave={create} onCancel={() => setEditForecast(false)} />}</>
                <HeaderWithAction title={t("forecast.title")} action={() => createNew()} actionText={t("common:actions.new")} />
                <div>
                    {forecasts?.map((item) => (
                        <ListItem key={item.id} item={item} title={item.title ?? ""} onEdit={startEdit} onDelete={deleteForecast} selected={forecast?.id === item.id} onClick={selectForecast} />
                    ))}
                    {(!forecasts || forecasts.length === 0) && <div className="italic pt-2 text-center">{t("forecast.noForecasts")}</div>}
                </div>
            </Section>
        </ContentContainer>
    );
};

export default Forecast;
