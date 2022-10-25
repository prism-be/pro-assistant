import styles from '../styles/components/popin.module.scss'

interface Props {
    children: JSX.Element;
}

export const Popin = (props: Props) => {
    return <div className={styles.popin}>
        {props.children}
    </div>
}

export const Popup = (props: Props) => {
    return <div className={styles.popup}>
        <div>
            {props.children}
        </div>
    </div>
}