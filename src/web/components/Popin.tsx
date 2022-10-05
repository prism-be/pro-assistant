import styles from '../styles/components/popin.module.scss'

interface Props {
    children: JSX.Element;
}

const Popin = (props: Props) => {
    return <div className={styles.panel}>
        {props.children}
    </div>
}

export default Popin;