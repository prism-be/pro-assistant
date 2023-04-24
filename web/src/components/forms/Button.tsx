import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<any>;
    secondary?: boolean;
    submit?: boolean;
    className?: string;
}

const Button = ({ text, onClick, secondary, className, submit }: Props) => {
    return (
        <>
            {secondary === true && (
                <button
                    type={"button"}
                    className={
                        "w-full bg-white text-primary px-2 py-1 hover:bg-gray-100 transition duration-100 border border-secondary" +
                        " " +
                        className
                    }
                    onClick={onClick}
                >
                    {text}
                </button>
            )}
            {!secondary && (
                <button
                    type={submit === true ? "submit" : "button"}
                    className={
                        "w-full bg-primary text-white px-2 py-1 hover:bg-secondary transition duration-100 border border-secondary" +
                        " " +
                        className
                    }
                    onClick={onClick}
                >
                    {text}
                </button>
            )}
        </>
    );
};

export default Button;
