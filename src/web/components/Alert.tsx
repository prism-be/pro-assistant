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

        const classes = [];

        const alertTypeClass = {
            [AlertType.Success]: "border-green-800 text-green-800",
            [AlertType.Error]: "border-red-800 text-red-800",
            [AlertType.Info]: "border-primary text-primary",
            [AlertType.Warning]: "border-orange-800 text-orange-800"
        }

        classes.push(alertTypeClass[alert.type]);

        if (alert.fade) {
            classes.push('fade');
        }

        return classes.join(' ');
    }

    if (!alerts.length) return null;

    return (
        <div className={"fixed top-20 left-0 right-0 p-4"}>
            {alerts.map((alert, index) =>
                <div key={index} className={"m-auto max-w-screen-md bg-white p-4 border rounded shadow relative mb-2 " + cssClasses(alert)}>
                    <a className={"w-6 cursor-pointer block absolute top-2 right-2 " + cssClasses(alert)} onClick={() => removeAlert(alert)}>
                        <XCircleIcon/>
                    </a>
                    <span dangerouslySetInnerHTML={{__html: alert.message}}></span>
                </div>
            )}
        </div>
    );
}