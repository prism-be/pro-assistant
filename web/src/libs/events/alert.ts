import {Subject} from "rxjs";
import {filter} from "rxjs/operators";

export const AlertType = {
    Success: "Success",
    Error: "Error",
    Info: "Info",
    Warning: "Warning",
};

interface Alert {
    id?: string;
    autoClose?: boolean;
    type: "Success" | "Error" | "Info" | "Warning";
    message: string;
}

const alertSubject = new Subject();
const defaultId = "default-alert";

// enable subscribing to alerts observable
export const onAlert = (id = defaultId) => {
    return alertSubject.asObservable().pipe(filter<any>((x) => x?.id === id));
};

// convenience methods
export const alertSuccess = (message: string, options?: any) => {
    alert({ ...options, type: AlertType.Success, message });
};

export const alertError = (message: string, options: any) => {
    alert({ ...options, type: AlertType.Error, message });
};

export const alertInfo = (message: string, options: any) => {
    alert({ ...options, type: AlertType.Info, message });
};

export const alertWarn = (message: string, options: any) => {
    alert({ ...options, type: AlertType.Warning, message });
};

// core alert method
export const alert = (alert: Alert) => {
    alert.id = alert.id || defaultId;
    alert.autoClose = alert.autoClose === undefined ? true : alert.autoClose;
    alertSubject.next(alert);
};

// clear alerts
export const clear = (id = defaultId) => {
    alertSubject.next({ id });
};
