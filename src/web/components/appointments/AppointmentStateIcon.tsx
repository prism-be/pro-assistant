﻿import {CheckCircleIcon} from '@heroicons/react/24/outline';
import {CheckCircleIcon as CheckCircleIconSolid} from '@heroicons/react/24/solid';

interface Props
{
    payment: number;
    state: number;
    backgroundColor?: string;
}

export const AppointmentStateIcon = ({payment, state, backgroundColor}: Props) => {
    return <>
        {state === 10 && payment === 0 && <div className={"styles.iconSmall"} style={{backgroundColor: backgroundColor ?? ""}}><CheckCircleIcon /></div>}
        {state === 10 && payment > 0 && <div className={"styles.iconSmall"} style={{backgroundColor: backgroundColor ?? ""}}><CheckCircleIconSolid /></div>}
    </>
}