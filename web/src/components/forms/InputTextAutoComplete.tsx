import {FieldValues, UseFormRegister} from "react-hook-form";
import {UseFormSetValue} from "react-hook-form/dist/types/form";
import {onlyUnique} from "@/libs/text";
import {useState} from "react";

interface Props {
    label: string;
    name: string;
    type: string;
    required?: boolean;
    register: UseFormRegister<FieldValues>;
    setValue: UseFormSetValue<FieldValues>;
    suggestions: string[];
    error?: any;
    autoCapitalize?: boolean;
    className?: string;
    onChange?: (value: string) => void;
}

const InputTextAutoComplete = ({
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
                                   suggestions,
                               }: Props) => {
    const valueChanged = (e: { target: { value: any } }) => {
        if (autoCapitalize === true) {
            setValue(name, e.target.value.charAt(0).toUpperCase() + e.target.value.slice(1));
        }

        if (onChange) {
            onChange(e.target.value);
        }
        
        suggest(e.target.value);
    };

    const [currentSuggestions, setCurrentSuggestions] = useState<string[]>([]);

    function suggest(text: string) {

        if (text.length < 3) {
            setCurrentSuggestions([]);
            return;
        }

        let foundSuggestions = suggestions?.filter((s) => s.toUpperCase().startsWith(text.toUpperCase()));
        console.log(foundSuggestions);

        foundSuggestions = foundSuggestions?.filter(onlyUnique);

        if (foundSuggestions?.length == 1 && foundSuggestions[0].toUpperCase() === text.toUpperCase())
        {
            setCurrentSuggestions([]);
            return;
        }

        setCurrentSuggestions(foundSuggestions ?? []);
    }
    
    function selectSuggestion(text: string)
    {
        setValue(name, text);
        setCurrentSuggestions([]);
    }

    return (
        <>
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
            {currentSuggestions.length > 0 && <div className={"block"}>
                <div className={"bg-gray-100 p-1"}>
                    {currentSuggestions.map((suggestion) => <div key={suggestion} className={"cursor-pointer hover:bg-gray-200 p-1"}
                                                          onClick={() => { selectSuggestion(suggestion); }}>
                        {suggestion}
                    </div>)}
                </div>
            </div>}
        </>
    );
};

export default InputTextAutoComplete;
