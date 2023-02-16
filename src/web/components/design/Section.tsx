interface Props {
    children: JSX.Element | JSX.Element[];
}

const Section = ({children}: Props) => {
    return <section className={"m-2 lg:p-4 lg:border lg:shadow"}>
        {children}
    </section>
}

export default Section;