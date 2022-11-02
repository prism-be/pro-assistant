import useKeyboardJs from "react-use/lib/useKeyboardJs";
import {useEffect, useState} from "react";
import {onPopup, popupNewMeeting, PopupParameters} from "../../lib/events/globalPopups";
import {MeetingPopup} from "./MeetingPopup";
import {useKeyPressEvent} from "react-use";

export const GlobalPopupsContainer = () => {

    const [isNewMeetingPressed, isNewMeeting] = useKeyboardJs("alt + n")
    useEffect(() => {
        if (isNewMeetingPressed) {
            isNewMeeting?.preventDefault();
            popupNewMeeting()
        }
    }, [isNewMeetingPressed]);
    
    const [displayNewMeeting, setDisplayNewMeeting] = useState(false);
    const [meetingData, setMeetingData] = useState<any>();

    useEffect(() => {
        onPopup().subscribe((popup: PopupParameters) => {
            switch (popup.type) {
                case "new-meeting":
                    setMeetingData(popup.data ?? null);
                    setDisplayNewMeeting(true);
                    break;
            }
        })
    }, []);

    useKeyPressEvent('Escape', () => {
        setDisplayNewMeeting(false);
    })

    return <div>
        {displayNewMeeting && <>
            <MeetingPopup data={meetingData} hide={() => setDisplayNewMeeting(false)} />
        </>}
    </div>
}