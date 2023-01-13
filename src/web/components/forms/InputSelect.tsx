import styles from "../../styles/components/forms/input.select.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";

interface Option {
    value: string;
    text: string;
}

interface Props {
    label: string;
    name: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    error?: any;
    className?: string;
    options: Option[];
    onChange?: (value: string) => void;
}


const InputSelect = ({label, name, required, register, error, className, options, onChange}: Props) => {

    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <select
            className={error ? styles.errorInput : styles.input}
            {...register(name, {
                required, onChange: (e) => {
                    if (onChange) {
                        onChange(e.target.value);
                    }
                }
            })}>
            {options.map(o => <option key={o.value} value={o.value}>{o.text}</option>)}
        </select>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputSelect;
