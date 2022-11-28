import {NextPage} from "next";
import getConfig from "next/config";

const { serverRuntimeConfig, publicRuntimeConfig } = getConfig()

const Temp: NextPage = () => {
    
    return <ul>
        <li>{process.env.NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING}</li>
        <li>{publicRuntimeConfig.aiConnectionString}</li>
    </ul>
}

export default Temp;