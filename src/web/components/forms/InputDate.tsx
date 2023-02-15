import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";
import {add, format, parse} from "date-fns";

interface Props {
    label: string;
    name: string;
    type: string;
    required: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    error: any;
    className?: string;
}


const InputDate = ({label, name, type, required, register, error, setValue, className}: Props) => {

    let thinkTimeout: any;

    const autoFormatContent = (e: { target: { value: any; }; }) => {

        if (thinkTimeout) {
            clearTimeout(thinkTimeout);
        }

        thinkTimeout = setTimeout(() => formatDate(e.target.value), 250)
    }

    const formatDate = (data: string) => {
        if (data.length < 5) {
            return;
        }

        const value = parse(data, "d/M/yy", add(new Date(), {
            years: -50
        }));
        if (value.toString() !== "Invalid Date") {
            setValue(name, format(value, "dd/MM/yyyy"));
        }
    }

    return <div className={"block" + " " + className}>
        <label className={"block"}>{label} {required && " *"} </label>
        <input
            className={"w-full block p-2 outline-0 " + (error ? "border border-red-400" : "border border-gray-100")}
            type={type}
            {...register(name, {
                required, onChange: (e) => {
                    autoFormatContent(e)
                }
            })}/>
        {error?.message && <p className={"styles.errorMessage"}>{error.message}</p>}
    </div>
}

export default InputDate;
