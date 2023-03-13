interface Props {
    children: JSX.Element;
}

export const Popin = (props: Props) => {
    return <div className={"border max-w-lg text-center p-2 mx-auto mt-2"}>
        {props.children}
    </div>
}

export const Popup = (props: Props) => {
    return <div className={"fixed top-0 left-0 right-0 bottom-0 h-full w-full bg-gray-100 bg-opacity-50"}>
        <div className={"border w-full max-w-3xl mt-20 p-4 mx-auto mt-2 bg-white"}>
            {props.children}
        </div>
    </div>
}