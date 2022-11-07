import styles from "../../styles/components/forms/input.text.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";

interface Props {
    label: string;
    name: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    error?: any;
    className?:string;
    onChange?: (value: string) => void;
}


const TextArea = ({label, name, required, register, error, className, onChange}: Props) => {

    const valueChanged = (e: { target: { value: any; }; }) => {
        if (onChange)
        {
            onChange(e.target.value);
        }
    }
    
    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <textarea
            className={error ? styles.errorInput : styles.input}
            {...register(name, {required, onChange: (e) => { valueChanged(e) }})}/>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default TextArea;
