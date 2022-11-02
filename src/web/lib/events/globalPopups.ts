import { Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

export interface PopupParameters
{
    id: string;
    type: 'new-meeting';
    data: any;
}

const popupSubject = new Subject();
const defaultId = 'default-popup';

// enable subscribing to alerts observable
export const onPopup = (id = defaultId) => {
    return popupSubject.asObservable().pipe(filter<any>(x => (x?.id === id)));
}

// convenience methods
export const popupNewMeeting = (options?: any) => {
    popup({ ...options, type: 'new-meeting' });
}

// core alert method
export const popup = (popup: PopupParameters) => {
    popup.id = popup.id || defaultId;
    popupSubject.next(popup);
}

// clear alerts
export const clearPopup = (id = defaultId) => {
    popupSubject.next({ id });
}