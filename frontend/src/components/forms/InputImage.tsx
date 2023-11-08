import {FieldValues, UseFormRegister, UseFormSetValue} from "react-hook-form";
import {useEffect, useState} from "react";

interface Props {
    label: string;
    name: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    initialPreview?: string;
    error?: any;
    className?: string;
    onChange?: (value: string | ArrayBuffer | null) => void;
}

const InputImage = ({
    label,
    name,
    required,
    register,
    error,
    setValue,
    className,
    onChange,
    initialPreview,
}: Props) => {
    const [preview, setPreview] = useState<any>(initialPreview);

    useEffect(() => {
        setPreview(initialPreview);
    }, [initialPreview]);

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
    };

    const valueChanged = (e: any) => {
        if (onChange) {
            onChange(e.target.value);
        }
    };

    return (
        <div className={"block" + " " + className}>
            <label className={"block"}>
                {label} {required && " *"}{" "}
            </label>
            <div
                className={"w-full block p-2 outline-0 " + (error ? "border border-red-400" : "border border-gray-200")}
            >
                {preview && <img className={"h-32 m-auto"} src={preview} alt={"logo"} />}

                <input
                    type={"hidden"}
                    {...register(name, {
                        required,
                        onChange: (e) => {
                            valueChanged(e);
                        },
                    })}
                />

                <input type="file" accept="image/*" onChange={(e) => fileChanged(e)} />
                {error?.message && <p className={"text-red-400"}>{error.message}</p>}
            </div>
        </div>
    );
};

export default InputImage;
