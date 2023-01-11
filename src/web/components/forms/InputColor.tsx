import {FieldValues} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";
import styles from "../../styles/components/forms/input.color.module.scss";
import {useEffect, useState} from "react";
import useTranslation from "next-translate/useTranslation";
import { PencilSquareIcon } from '@heroicons/react/24/outline';

interface Props {
    label: string;
    name: string;
    setValue: UseFormSetValue<FieldValues>;
    error?: any;
    className?: string;
    onChange?: (value: string) => void;
    initialColor?: string;
}

const InputColor = ({label, name, error, className, setValue, onChange, initialColor}: Props) => {

    const {t} = useTranslation("common");
    const [color, setColor] = useState<string>(initialColor ?? "#000000");
    const [displayPicker, setDisplayPicker] = useState<boolean>(false);
    let foreColor = "#FFFFFF";
    
    useEffect(() => {
        setColor(initialColor ?? "#000000");
    }, [initialColor]);

    function setCustomColor() {
        const newColor = prompt(t("alerts.customColor"), color);

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
    ]

    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label}</label>
        <div className={styles.pill} style={{backgroundColor: color, color: foreColor}}>
            <div className={styles.refresh} onClick={() => setDisplayPicker(!displayPicker)}>
                <PencilSquareIcon/>
            </div>
            <div  className={styles.colorName} onClick={() => setCustomColor()}>
                {color}
            </div>
        </div>

        {displayPicker && <div className={styles.colorPicker}>
            {
                colors.map(c => <div key={c} className={styles.colorPickerChoice} style={{backgroundColor: c}} onClick={() => {
                    changeColor(c);
                    setDisplayPicker(false);
                }}></div>)
            }
        </div>}

        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputColor;