import { jsPDF } from "jspdf";

export const generateReceipt = (meetingId: string) => {
    let pdf = new jsPDF();
    
    pdf.output('dataurlnewwindow', { filename: 'receipt-' + meetingId + '.pdf' });
}