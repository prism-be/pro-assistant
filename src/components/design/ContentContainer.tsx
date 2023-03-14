import Menu from "./Menu";
import Header from "./Header";

interface Props {
  children: JSX.Element | JSX.Element[];
}

const ContentContainer = (props: Props) => {
  return (
    <div>
      <Header />
      <div className={"p-0 m-0 border-box relative w-full"}>
        <Menu />
        <div className={"md:ml-64"}>{props.children}</div>
      </div>
    </div>
  );
};

export default ContentContainer;
