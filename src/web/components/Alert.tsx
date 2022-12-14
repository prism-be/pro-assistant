import styles from '../styles/components/alert.module.scss';

import {useEffect, useRef, useState} from "react";
import {useRouter} from "next/router";
import {AlertType, clear, onAlert} from "../lib/events/alert";
import {XCircleIcon} from '@heroicons/react/24/outline';

interface Props {
    id?: string;
    fade?: boolean;
}

export const Alert = ({id, fade}: Props) => {
    const mounted = useRef(false);
    const router = useRouter();
    const [alerts, setAlerts] = useState<any[]>([]);

    useEffect(() => {
        mounted.current = true;

        const subscription = onAlert(id)
            .subscribe(alert => {
                if (!alert.message) {
                    setAlerts([]);
                } else {
                    // add alert to array with unique id
                    alert.itemId = Math.random();
                    setAlerts(alerts => ([...alerts, alert]));

                    // auto close alert if required
                    if (alert.autoClose) {
                        setTimeout(() => removeAlert(alert), 3000);
                    }
                }
            });

        // clear alerts on location change
        const clearAlerts = () => clear(id);
        router.events.on('routeChangeStart', clearAlerts);

        // clean up function that runs when the component unmounts
        return () => {
            mounted.current = false;

            // unsubscribe to avoid memory leaks
            subscription.unsubscribe();
            router.events.off('routeChangeStart', clearAlerts);
        };
    }, []);

    const removeAlert = (alert: any) => {
        if (!mounted.current) return;

        if (fade) {
            // fade out alert
            setAlerts(alerts => alerts.map(x => x.itemId === alert.itemId ? {...x, fade: true} : x));

            // remove alert after faded out
            setTimeout(() => {
                setAlerts(alerts => alerts.filter(x => x.itemId !== alert.itemId));
            }, 250);
        } else {
            // remove alert
            setAlerts(alerts => alerts.filter(x => x.itemId !== alert.itemId));
        }
    };

    const cssClasses = (alert: any) => {
        if (!alert) return;

        const classes = [styles.toaster];

        const alertTypeClass = {
            [AlertType.Success]: styles.toasterSuccess,
            [AlertType.Error]: styles.toasterError,
            [AlertType.Info]: styles.toasterInfo,
            [AlertType.Warning]: styles.toasterWarning
        }

        classes.push(alertTypeClass[alert.type]);

        if (alert.fade) {
            classes.push('fade');
        }

        return classes.join(' ');
    }

    if (!alerts.length) return null;

    return (
        <div className={styles.toasterContainer}>
            {alerts.map((alert, index) =>
                <div key={index} className={cssClasses(alert)}>
                    <a className="close" onClick={() => removeAlert(alert)}>
                        <XCircleIcon/>
                    </a>
                    <span dangerouslySetInnerHTML={{__html: alert.message}}></span>
                </div>
            )}
        </div>
    );
}