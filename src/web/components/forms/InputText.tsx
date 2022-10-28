import styles from "../../styles/components/forms/input.text.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";

interface Props {
    label: string;
    name: string;
    type: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    error?: any;
    autoCapitalize?: boolean;
    className?:string;
    onChange?: (value: string) => void;
}


const InputText = ({label, name, type, required, register, error, autoCapitalize, setValue, className, onChange}: Props) => {

    const valueChanged = (e: { target: { value: any; }; }) => {
        if (autoCapitalize === true)
        {
            setValue(name, e.target.value.charAt(0).toUpperCase() + e.target.value.slice(1));
        }
        
        if (onChange)
        {
            onChange(e.target.value);
        }
    }
    
    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <input
            className={error ? styles.errorInput : styles.input}
            type={type}
            {...register(name, {required, onChange: (e) => { valueChanged(e) }})}/>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputText;
