import { Forecast } from "@/libs/models";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import Button from "../forms/Button";
import InputText from "../forms/InputText";
import { Popup } from "../Pops";

interface Props {
    onSave: (data: Forecast) => void;
    onCancel: () => void;
    forecast?: Forecast;
}

export const ForecastPopup = ({ forecast, onCancel, onSave }: Props) => {
    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        getValues,
    } = useForm();

    const { t } = useTranslation("accounting");

    useEffect(() => {
        if (forecast) {
            setValue("title", forecast.title);
        }
    }, [forecast, setValue]);

    function onSubmit() {
        const data: Forecast = {
            id: forecast?.id ?? "",
            title: getValues()["title"],
        };

        onSave(data);
    }

    return (
        <Popup>
            <form>
                <div className={"grid grid-cols-2 gap-2"}>
                    <div className={"col-span-2"}>
                        <InputText label={t("forecast.popup.title")} name={"title"} type={"text"} required={true} register={register} setValue={setValue} error={errors.title} />
                    </div>

                    <Button text={t("common:actions.cancel")} onClick={onCancel} secondary={true} />
                    <Button text={t("common:actions.save")} onClick={handleSubmit(onSubmit)} />
                </div>
            </form>
        </Popup>
    );
};
