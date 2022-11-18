import styles from '../../styles/components/design/section.module.scss';

interface Props {
    children: JSX.Element;
}

const Section = ({children}: Props) => {
    return <section className={styles.section}>
        {children}
    </section>
}

export default Section;