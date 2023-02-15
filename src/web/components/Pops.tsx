interface Props {
    children: JSX.Element;
}

export const Popin = (props: Props) => {
    return <div className={"border max-w-lg text-center p-2 mx-auto mt-2"}>
        {props.children}
    </div>
}

export const Popup = (props: Props) => {
    return <div className={"border max-w-lg text-center p-2 mx-auto mt-2"}>
        <div>
            {props.children}
        </div>
    </div>
}