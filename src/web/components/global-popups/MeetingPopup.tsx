import {Popup} from "../Pops";

interface Props {
    meetingId?: string;
}

export const MeetingPopup = ({meetingId}: Props) => {
    return <Popup>
        <>
            <h1>Hello !</h1>
        </>
    </Popup>
}