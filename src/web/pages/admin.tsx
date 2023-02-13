import {NextPage} from "next";
import Button from "../components/forms/Button";
import {putData} from "../lib/ajaxHelper";

const Admin: NextPage = () => {
    return <div>
        <Button text={"Rebuild"} onClick={async () => {
            await putData("/admin/rebuild", {});
        }}/>
    </div>
}

export default Admin;