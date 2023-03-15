import { CheckCircleIcon } from "@heroicons/react/24/outline";
import { CheckCircleIcon as CheckCircleIconSolid } from "@heroicons/react/24/solid";

interface Props {
    payment: number;
    state: number;
    backgroundColor?: string;
}

export const AppointmentStateIcon = ({ payment, state, backgroundColor }: Props) => {
    return (
        <>
            {state === 10 && payment === 0 && (
                <div className={"w-4 absolute top-1 right-1"} style={{ backgroundColor: backgroundColor ?? "" }}>
                    <CheckCircleIcon />
                </div>
            )}
            {state === 10 && payment > 0 && (
                <div className={"w-4 absolute top-1 right-1"} style={{ backgroundColor: backgroundColor ?? "" }}>
                    <CheckCircleIconSolid />
                </div>
            )}
        </>
    );
};
