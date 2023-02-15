interface Props {
    children: JSX.Element | JSX.Element[];
}

const Section = ({children}: Props) => {
    return <section className={"m-2"}>
        {children}
    </section>
}

export default Section;