import {FieldValues, UseFormRegister} from "react-hook-form";

interface Props {
    label: string;
    name: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    error?: any;
    className?: string;
    onChange?: (value: string) => void;
}


const TextArea = ({label, name, required, register, error, className, onChange}: Props) => {

    const valueChanged = (e: { target: { value: any; }; }) => {
        if (onChange) {
            onChange(e.target.value);
        }
    }

    return <div className={"block" + " " + className}>
        <label className={"block"}>{label} {required && " *"} </label>
        <textarea
            className={"w-full block p-2 outline-0 h-48 " + (error ? "border border-red-400" : "border border-gray-200")}
            {...register(name, {
                required, onChange: (e) => {
                    valueChanged(e)
                }
            })}/>
        {error?.message && <p className={"text-red-400"}>{error.message}</p>}
    </div>
}

export default TextArea;
