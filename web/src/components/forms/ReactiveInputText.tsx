import {ObservablePrimitiveChildFns} from "@legendapp/state";
import {Reactive} from "@legendapp/state/react";

export interface Props {
    value: ObservablePrimitiveChildFns<string>;
    label: string;
    required?: boolean;
    type?: 'text' | 'password';
    className?: string;
    autoCapitalize?: boolean;
}

const ReactiveInputText = ({value, label, required, type, className, autoCapitalize}: Props) => {

    value.onChange(() => {
        if (autoCapitalize === true) {
            const actual = value.get();
            value.set(actual.charAt(0).toUpperCase() + actual.slice(1));
        }
    });

    return <div className={"block" + " " + className}>
        <label className={"block"}>
            {label} {required && " *"}{" "}
        </label>
        <Reactive.input $value={value} type={type ?? "text"}
                        className={"w-full block p-2 outline-0 border border-gray-200"} autoComplete="off"/>
    </div>
}

export default ReactiveInputText;