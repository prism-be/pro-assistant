import { FieldValues, UseFormRegister } from "react-hook-form";
import { UseFormSetValue } from "react-hook-form/dist/types/form";

interface Props {
    label: string;
    name: string;
    type: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    error?: any;
    autoCapitalize?: boolean;
    className?: string;
    onChange?: (value: string) => void;
}

const InputText = ({
    label,
    name,
    type,
    required,
    register,
    error,
    autoCapitalize,
    setValue,
    className,
    onChange,
}: Props) => {
    const valueChanged = (e: { target: { value: any } }) => {
        if (autoCapitalize === true) {
            setValue(name, e.target.value.charAt(0).toUpperCase() + e.target.value.slice(1));
        }

        if (onChange) {
            onChange(e.target.value);
        }
    };

    return (
        <div className={"block" + " " + className}>
            <label className={"block"}>
                {label} {required && " *"}{" "}
            </label>
            <input
                className={"w-full block p-2 outline-0 " + (error ? "border border-red-400" : "border border-gray-200")}
                type={type}
                {...register(name, {
                    required,
                    onChange: (e) => {
                        valueChanged(e);
                    },
                })}
                autoComplete="off"
            />
            {error?.message && <p className={"text-red-400"}>{error.message}</p>}
        </div>
    );
};

export default InputText;
