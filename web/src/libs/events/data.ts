import { Subject } from "rxjs";
import { filter } from "rxjs/operators";

export interface DataUpdated {
    type: "appointment";
}

const dataUpdatedSubject = new Subject();

export const onDataUpdated = (dataObserved: DataUpdated) => {
    return dataUpdatedSubject.asObservable().pipe(filter<any>((x) => x?.type === dataObserved.type));
};

export const dataUpdated = (data: DataUpdated) => {
    dataUpdatedSubject.next(data);
};
