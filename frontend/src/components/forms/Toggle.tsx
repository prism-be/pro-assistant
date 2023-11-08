import { useState } from "react";

export interface Props {
    value: boolean;
    className?: string;
    text?: string;
    onChange?: (value: boolean) => void;
}

export const Toggle = ({ text, value, className, onChange }: Props) => {

    const [checked, setChecked] = useState<boolean>(value);

    function handleChange() {
        setChecked(!checked);
        onChange && onChange(!checked);
    }

    return <>
    
        <label className={"relative inline-flex items-center cursor-pointer " + className}>
        <input type="checkbox" checked={checked} onChange={handleChange} className="sr-only peer" />
        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-light dark:peer-focus:ring-light rounded-full peer
        dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px]
            after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all 
            dark:border-gray-600 peer-checked:bg-secondary"></div>
        <span className="ml-3 text-sm font-medium text-gray-900 dark:text-gray-300">{text}</span>
        </label>

    </>
}