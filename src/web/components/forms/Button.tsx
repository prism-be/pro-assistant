import styles from "../../styles/components/forms/button.module.scss";

import {MouseEventHandler} from "react";

interface Props {
    text: string;
    onClick?: MouseEventHandler<HTMLButtonElement>
}

const Button = ({text, onClick}: Props) => {
    return <>
        <button className={styles.button} onClick={onClick}>
            {text}
        </button>
    </>
}

export default Button;