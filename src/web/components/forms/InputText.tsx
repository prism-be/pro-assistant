import styles from "../../styles/components/forms/input.text.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";

interface Props {
    label: string;
    name: string;
    type: string;
    required: boolean;
    register: UseFormRegister<FieldValues>;
    error: any;
}


const InputText = ({label, name, type, required, register, error}: Props) => {

    return <div className={styles.container}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <input
            className={error ? styles.errorInput : styles.input}
            type={type}
            {...register(name, {required})}/>
        {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
    </div>
}

export default InputText;
