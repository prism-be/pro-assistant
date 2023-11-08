import { putData} from "@/libs/http";
import {alertSuccess} from "@/libs/events/alert";

const Admin = () => {
    async function aggregateEvents() {
        if (confirm('Are you sure ? This may take time !')) {
            await putData('/maintenance/rebuild', {});
            alertSuccess('All projections have been rebuilt !');
        }
    }

    async function rebuildAccountingPeriods() {
        if (confirm('Are you sure ? This may take time !')) {
            await putData('/maintenance/rebuild-accounting-periods', {});
            alertSuccess('All periods have been rebuilt !');
        }
    }

    return (
            <div className={"p-5"}>
                <h1>Admin - Take care with all these actions !!!</h1>
                <div className={"pt-5"}>
                    <button onClick={aggregateEvents} className={"bg-primary text-white p-2"}>Rebuild Projections</button>
                </div>

                <div className={"pt-5"}>
                    <button onClick={rebuildAccountingPeriods} className={"bg-primary text-white p-2"}>Rebuild Accounting Periods</button>
                </div>
            </div>
    );
};

export default Admin;
