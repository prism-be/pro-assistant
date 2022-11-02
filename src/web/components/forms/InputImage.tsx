import styles from "../../styles/components/forms/input.image.module.scss"

import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormGetValues, UseFormSetValue} from "react-hook-form/dist/types/form";
import {useEffect, useState} from "react";

interface Props {
    label: string;
    name: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    initialPreview?: string | undefined;
    error?: any;
    className?: string;
    onChange?: (value: string | ArrayBuffer | null) => void;
}


const InputImage = ({label, name, required, register, error, setValue, className, onChange, initialPreview}: Props) => {
    const [preview, setPreview] = useState<any>(initialPreview);
    
    useEffect(() => {
        setPreview(initialPreview);
    }, [initialPreview])

    const fileChanged = (e: any) => {
        const file = e.target.files[0];
        if (file) {
            const fileReader = new FileReader();
            fileReader.addEventListener("load", function () {
                setValue(name, fileReader.result);
                setPreview(fileReader.result);

            });
            fileReader.readAsDataURL(file);
        }
    }

    const valueChanged = (e: any) => {
        if (onChange) {
            onChange(e.target.value);
        }
    }

    return <div className={styles.container + " " + className}>
        <label className={styles.label}>{label} {required && " *"} </label>
        <div className={styles.containerInput}>
            {preview && <img className={styles.preview} src={preview} alt={"logo"}/>}
            
            <input type={"hidden"} {...register(name, {
                required, onChange: (e) => {
                    valueChanged(e)
                }
            })} />

            <input
                className={error ? styles.errorInput : styles.input}
                type="file"
                accept="image/*"
                onChange={(e) => fileChanged(e)}
            />
            {error?.message && <p className={styles.errorMessage}>{error.message}</p>}
        </div>
    </div>
}

export default InputImage;
