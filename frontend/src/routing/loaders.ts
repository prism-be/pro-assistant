import {LoaderFunctionArgs} from "react-router-dom";

export async function passThroughLoader({ params }: LoaderFunctionArgs) {
    return params;
}