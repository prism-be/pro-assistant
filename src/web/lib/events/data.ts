import {Subject} from "rxjs";
import {filter} from "rxjs/operators";

export interface dataUpdated
{
    type: 'appointment';
}

const dataUpdatedSubject = new Subject();

export const onDataUpdated = (dataObserved: dataUpdated) => {
    return dataUpdatedSubject.asObservable().pipe(filter<any>(x => (x?.type === dataObserved.type)));
}

export const dataUpdated = (data: dataUpdated) => {
    dataUpdatedSubject.next(data);
}