import {ObservablePrimitiveChildFns} from "@legendapp/state";
import {Reactive} from "@legendapp/state/react";
import {add, format, parse} from "date-fns";

export interface Props {
    value: ObservablePrimitiveChildFns<string>;
    label: string;
    required?: boolean;
    className?: string;
}

const ReactiveInputDate = ({value, label, required, className}: Props) => {

    let thinkTimeout: any;
    value.onChange(() => {
        if (thinkTimeout) {
            clearTimeout(thinkTimeout);
        }

        thinkTimeout = setTimeout(() => formatDate(), 500);
    });

    function formatDate() {
        if (value.get().length < 5) {
            return;
        }

        const parsed = parse(
            value.get(),
            "d/M/yy",
            add(new Date(), {
                years: -50,
            })
        );

        if (parsed.toString() !== "Invalid Date") {
            value.set(format(parsed, "dd/MM/yyyy"));
        }
    }

    return <div className={"block" + " " + className}>
        <label className={"block"}>
            {label} {required && " *"}{" "}
        </label>
        <Reactive.input $value={value} type="text" className={"w-full block p-2 outline-0 border border-gray-200"}
                        autoComplete="off"/>
    </div>
}

export default ReactiveInputDate;