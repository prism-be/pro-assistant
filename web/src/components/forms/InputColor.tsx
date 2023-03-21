import { FieldValues } from "react-hook-form";
import { UseFormSetValue } from "react-hook-form/dist/types/form";
import { useEffect, useState } from "react";
import {useTranslation} from "react-i18next";
import { PencilSquareIcon } from "@heroicons/react/24/outline";

interface Props {
    label: string;
    name: string;
    setValue: UseFormSetValue<FieldValues>;
    error?: any;
    className?: string;
    onChange?: (value: string) => void;
    initialColor?: string;
}

const InputColor = ({ label, name, error, className, setValue, onChange, initialColor }: Props) => {
    const { t } = useTranslation("common");
    const [color, setColor] = useState<string>(initialColor ?? "#000000");
    const [displayPicker, setDisplayPicker] = useState<boolean>(false);
    let foreColor = "#FFFFFF";

    useEffect(() => {
        setColor(initialColor ?? "#000000");
    }, [initialColor]);

    function setCustomColor() {
        const newColor = prompt(t("alerts.customColor").toString(), color);

        if (newColor && newColor !== "") {
            changeColor(newColor);
        }
    }

    function changeColor(color: string) {
        setColor(color);
        setValue(name, color);

        if (onChange) {
            onChange(color);
        }
    }

    const colors = [
        "#b9256e",
        "#db4035",
        "#fe9833",
        "#fad000",
        "#7ecd48",
        "#299439",
        "#6accbc",
        "#148ead",
        "#12aaf4",
        "#97c3ea",
        "#4072fe",
        "#894cff",
        "#794dce",
        "#af38eb",
        "#ea96ea",
        "#e15195",
        "#ff8d85",
        "#cdac92",
        "#b0784e",
        "#816244",
        "#5e432b",
        "#c7c7c7",
        "#808080",
        "#696969",
    ];

    return (
        <div className={"block relative" + " " + className}>
            <label className={"block"}>{label}</label>
            <div
                className={
                    "w-28 rounded-lg cursor-pointer text-center max-w-xs block p-2 outline-0 " +
                    (error ? "border border-red-400" : "border border-gray-200")
                }
                style={{ backgroundColor: color, color: foreColor }}
            >
                <div className={"w-8 m-auto"} onClick={() => setDisplayPicker(!displayPicker)}>
                    <PencilSquareIcon />
                </div>
                <div className={"text-sm"} onClick={() => setCustomColor()}>
                    {color}
                </div>
            </div>

            {displayPicker && (
                <div className={"grid grid-cols-8 md:absolute bg-white gap-2 p-2 border mt-2"}>
                    {colors.map((c) => (
                        <div
                            key={c}
                            className={"h-6 w-6 cursor-pointer m-auto"}
                            style={{ backgroundColor: c }}
                            onClick={() => {
                                changeColor(c);
                                setDisplayPicker(false);
                            }}
                        ></div>
                    ))}
                </div>
            )}

            {error?.message && <p className={"text-red-400"}>{error.message}</p>}
        </div>
    );
};

export default InputColor;
