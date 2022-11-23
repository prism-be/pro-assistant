import styles from "../../styles/components/forms/button.module.scss";

import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<any>;
    secondary?: boolean;
    submit?: boolean;
    className?:string;
}

const Button = ({text, onClick, secondary, className, submit}: Props) => {
    return <>
        {secondary === true && <button type={"button"} className={styles.buttonSecondary + " " + className} onClick={onClick}>{text}</button>}
        {!secondary && <button type={submit == true ? "submit" : "button"} className={styles.button + " " + className} onClick={onClick}>{text}</button>}
    </>
}

export default Button;