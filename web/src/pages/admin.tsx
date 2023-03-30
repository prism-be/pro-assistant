import {postData} from "@/libs/http";
import {alertSuccess} from "@/libs/events/alert";

const Admin = () => {
    async function aggregateEvents() {
        if (confirm('Are you sure ? You will lose all the history')) {
            await postData('/maintenance/rebuild-events', {});
            alertSuccess('All events have been aggregated');
        }
    }

    return (
            <div className={"p-5"}>
                <h1>Admin - Take care with all these actions !!!</h1>
                <div className={"pt-5"}>
                    <button onClick={aggregateEvents} className={"bg-primary text-white p-2"}>Aggregate all events</button>
                </div>
            </div>
    );
};

export default Admin;
