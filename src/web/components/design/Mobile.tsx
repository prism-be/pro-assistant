
export interface Props {
    visible?: boolean;
    children: JSX.Element | JSX.Element[];
    breakpoint: 'SM' | 'MD' | 'LG' | 'XL';
    className?: string;
}

const Mobile = ({visible, children, breakpoint, className}: Props) => {


    function getClassName() {
        if (visible) {
            switch (breakpoint) {
                case "SM":
                    return" styles.mobileDisplaySM";
                case "MD":
                    return "styles.mobileDisplayMD";
                case "LG":
                    return "styles.mobileDisplayLG";
                case "XL":
                    return "styles.mobileDisplayXL";
            }
        } else {
            switch (breakpoint) {
                case "SM":
                    return "styles.mobileHideSM";
                case "MD":
                    return "styles.mobileHideMD";
                case "LG":
                    return "styles.mobileHideLG";
                case "XL":
                    return "styles.mobileHideXL";
            }
        }
    }

    return <div className={getClassName() + " " + className}>
        {children}
    </div>
}

export default Mobile;