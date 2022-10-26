import styles from "../../styles/components/forms/input.text.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";

interface Props {
    label: string;
    name: string;
    type: string;
    required: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    error: any;
    autoCapitalize?: boolean;
    className?:string;
}


const InputText = ({label, name, type, required, register, error, autoCapitalize, setValue, className}: Props) => {

    const autoCapitalizeContent = (e: { target: { value: any; }; }) => {
        if (autoCapitalize === true)
        {
            setValue(name, e.target.value.charAt(0).toUpperCase() + e.target.value.slice(1));
        }
    }
    
    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <input
            className={error ? styles.errorInput : styles.input}
            type={type}
            {...register(name, {required, onChange: (e) => { autoCapitalizeContent(e) }})}/>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputText;
