import {
    InteractionRequiredAuthError,
    InteractionStatus,
} from "@azure/msal-browser";
import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";
import {useEffect, useState} from "react";

const ProtectedComponent = () => {
    const { instance, inProgress, accounts } = useMsal();
    const [apiData, setApiData] = useState(null);

    useEffect(() => {
        if (!apiData && inProgress === InteractionStatus.None) {
            const accessTokenRequest = {
                scopes: ["records.manage"],
                account: accounts[0],
            };
            instance
                .acquireTokenSilent(accessTokenRequest)
                .then((accessTokenResponse) => {
                    // Acquire token silent success
                    let accessToken = accessTokenResponse.accessToken;
                    
                    console.log(accessTokenResponse);
                    
                    // TODO
                    
                })
                .catch((error) => {
                    if (error instanceof InteractionRequiredAuthError) {
                        instance
                            .acquireTokenPopup(accessTokenRequest)
                            .then(function (accessTokenResponse) {
                                // Acquire token interactive success
                                let accessToken = accessTokenResponse.accessToken;
                                
                                console.log(accessTokenResponse);

                                // TODO
                            })
                            .catch(function (error) {
                                // Acquire token interactive failure
                                console.log(error);
                            });
                    }
                    console.log(error);
                });
        }
    }, [instance, accounts, inProgress, apiData]);

    return <p>Return your protected content here: {apiData}</p>;
}

export default ProtectedComponent;