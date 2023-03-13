import Button from "../forms/Button";
import React from "react";

interface Props {
    title: string;
    action: () => void;
    actionText: string;
}

export const HeaderWithAction = ({title, action, actionText}: Props) => {
    return <header className={"flex"}>
        <h1 className={"grow"}>{title}</h1>
        <div className={"w-64"}>
            <Button secondary={true} onClick={() => action()} text={actionText}/>
        </div>
    </header>
}