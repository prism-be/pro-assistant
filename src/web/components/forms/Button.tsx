import styles from "../../styles/components/forms/button.module.scss";

import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<any>,
    secondary?: boolean
}

const Button = ({text, onClick, secondary}: Props) => {
    return <>
        {secondary === true && <a className={styles.buttonSecondary} onClick={onClick}>{text}</a>}
        {!secondary && <button className={styles.button} onClick={onClick}>{text}</button>}
    </>
}

export default Button;