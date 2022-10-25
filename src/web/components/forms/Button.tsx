import styles from "../../styles/components/forms/button.module.scss";

import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<any>;
    secondary?: boolean;
    className?:string;
}

const Button = ({text, onClick, secondary, className}: Props) => {
    return <>
        {secondary === true && <a className={styles.buttonSecondary + " " + className} onClick={onClick}>{text}</a>}
        {!secondary && <button className={styles.button + " " + className} onClick={onClick}>{text}</button>}
    </>
}

export default Button;