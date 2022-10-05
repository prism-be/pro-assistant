import styles from "../../styles/components/design/loader.module.scss"

const Loader = () => {
    return <div className={styles.container}>
        <div className={styles.loader}>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
        </div>
    </div>
}

export default Loader;