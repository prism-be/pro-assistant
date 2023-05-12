import React from "react";

interface Props {
    title: string;
}

export const HeaderTitle = ({ title }: Props) => {
    return (
        <header className={"flex"}>
            <h1 className={"grow"}>{title}</h1>
        </header>
    );
};
