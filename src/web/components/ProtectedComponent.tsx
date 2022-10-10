import {useMsal} from "@azure/msal-react";
import {getData} from "../lib/ajaxHelper";
import useSWR from 'swr';

const ProtectedComponent = () => {
    const {instance, accounts} = useMsal();
    const {data} = useSWR('/api/authentication/user', (apiURL: string) => getData(apiURL, instance, accounts[0]))


    return <p>Return your protected content here: {JSON.stringify(data)}</p>;
}

export default ProtectedComponent;