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

const InputSelect = ({ label, name, required, register, error, className, options, onChange }: Props) => {
    return (
        <div className={"block" + " " + className}>
            <label className={"block"}>
                {label} {required && " *"}{" "}
            </label>
            <select
                className={"w-full block p-2 outline-0 " + (error ? "border border-red-400" : "border border-gray-200")}
                {...register(name, {
                    required,
                    onChange: (e) => {
                        if (onChange) {
                            onChange(e.target.value);
                        }
                    },
                })}
            >
                {options.map((o) => (
                    <option key={o.value} value={o.value}>
                        {o.text}
                    </option>
                ))}
            </select>
            {error?.message && <p className={"text-red-400"}>{error.message}</p>}
        </div>
    );
};

export default InputSelect;
