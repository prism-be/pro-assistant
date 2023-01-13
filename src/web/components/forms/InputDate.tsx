import styles from "../../styles/components/forms/input.text.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";
import {add, format, parse} from "date-fns";

interface Props {
    label: string;
    name: string;
    type: string;
    required: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    error: any;
}


const InputDate = ({label, name, type, required, register, error, setValue}: Props) => {

    let thinkTimeout: any;

    const autoFormatContent = (e: { target: { value: any; }; }) => {

        if (thinkTimeout) {
            clearTimeout(thinkTimeout);
        }

        thinkTimeout = setTimeout(() => formatDate(e.target.value), 250)
    }

    const formatDate = (data: string) => {
        if (data.length < 5) {
            return;
        }

        const value = parse(data, "d/M/yy", add(new Date(), {
            years: -50
        }));
        if (value.toString() !== "Invalid Date") {
            setValue(name, format(value, "dd/MM/yyyy"));
        }
    }

    return <div className={styles.container}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <input
            className={error ? styles.errorInput : styles.input}
            type={type}
            {...register(name, {
                required, onChange: (e) => {
                    autoFormatContent(e)
                }
            })}/>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputDate;
