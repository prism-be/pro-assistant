export default function ContactDetails() {
    
    const contactId =  new URL(window.location.toString()).searchParams.get("contactId");
    
    return (
        <div>
            <h1  style={{viewTransitionName: "name" + contactId}}>Baudart</h1>
            <p>Contact ID: {contactId}</p>
        </div>
    );
}