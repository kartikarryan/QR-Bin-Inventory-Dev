import jsPDF from 'jspdf';

export interface PrintableLabel {
  partName: string;
  binCode: string;
  qrDataUrl: string;
}

// Fixed defaults — one size that works for any bin, no settings for the user to pick.
const LABEL_SIZE_MM = 60;
const COLUMNS = 3;
const GAP_MM = 4;
const MARGIN_MM = 10;

/**
 * Builds a ready-to-print PDF (fixed A4 sheet, 60mm labels, 3 per row) and triggers a browser
 * download — no print dialog, no settings. A manager hands this file straight to a print shop.
 */
export function downloadQrLabelsPdf(labels: PrintableLabel[], fileName: string): void {
  const doc = new jsPDF({ unit: 'mm', format: 'a4' });
  const pageWidth = doc.internal.pageSize.getWidth();
  const pageHeight = doc.internal.pageSize.getHeight();

  const colsPerPage = Math.max(1, Math.min(COLUMNS, Math.floor((pageWidth - MARGIN_MM * 2) / (LABEL_SIZE_MM + GAP_MM))));
  const rowsPerPage = Math.max(1, Math.floor((pageHeight - MARGIN_MM * 2) / (LABEL_SIZE_MM + GAP_MM)));
  const perPage = colsPerPage * rowsPerPage;

  const qrSize = LABEL_SIZE_MM * 0.6;

  labels.forEach((label, i) => {
    const posOnPage = i % perPage;
    if (i > 0 && posOnPage === 0) doc.addPage();

    const col = posOnPage % colsPerPage;
    const row = Math.floor(posOnPage / colsPerPage);
    const x = MARGIN_MM + col * (LABEL_SIZE_MM + GAP_MM);
    const y = MARGIN_MM + row * (LABEL_SIZE_MM + GAP_MM);

    doc.setDrawColor(0);
    doc.setLineWidth(0.5);
    doc.roundedRect(x, y, LABEL_SIZE_MM, LABEL_SIZE_MM, 2, 2);

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(10);
    doc.text(label.partName.toUpperCase(), x + LABEL_SIZE_MM / 2, y + 8, {
      align: 'center',
      maxWidth: LABEL_SIZE_MM - 6,
    });

    const qrX = x + (LABEL_SIZE_MM - qrSize) / 2;
    const qrY = y + 12;
    doc.addImage(label.qrDataUrl, 'PNG', qrX, qrY, qrSize, qrSize);

    doc.setFontSize(9);
    doc.text(`BIN ${label.binCode}`, x + LABEL_SIZE_MM / 2, qrY + qrSize + 7, { align: 'center' });
  });

  doc.save(fileName);
}
