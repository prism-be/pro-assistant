import {FieldValues, UseFormGetValues} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";
import styles from "../../styles/components/forms/input.color.module.scss";
import {useState} from "react";
import {Refresh} from "../icons/Icons";

interface Props {
    label: string;
    name: string;
    setValue: UseFormSetValue<FieldValues>;
    getValues: UseFormGetValues<FieldValues>;
    error?: any;
    className?: string;
    onChange?: (value: string) => void;
}

const InputColor = ({label, name, error, className, setValue, onChange, getValues}: Props) => {

    const [color, setColor] = useState<string>(getValues()[name] ?? "#000000");
    let foreColor = "#FFFFFF";

    function generateColor() {
        let newColor = "#";
        for (let i = 0; i < 6; i++) {
            newColor += Math.floor(Math.random() * 16).toString(16);
        }

        setColor(newColor);
        setValue(name, newColor);
        
        if (onChange) {
            onChange(newColor);
        }
    }

    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label}</label>
        <div className={styles.pill} style={{backgroundColor: color, color: foreColor}}>
            <div className={styles.refresh} onClick={() => generateColor()}>
                <Refresh/>
            </div>
            <div>
                {color}
            </div>
        </div>

        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputColor;