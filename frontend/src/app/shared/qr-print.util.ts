export interface PrintableLabel {
  partName: string;
  binCode: string;
  qrDataUrl: string;
}

export type PaperSize = 'A4' | 'Letter';

export interface PrintSettings {
  labelSizeMm: number;
  columns: number;
  paper: PaperSize;
}

const DEFAULT_SETTINGS: PrintSettings = { labelSizeMm: 60, columns: 3, paper: 'A4' };

/**
 * Prints via a dedicated blank window rather than an in-page print stylesheet — isolating the
 * label markup this way avoids having to fight the app shell (header/sidebar) out of the printout.
 * Labels lay out as a real grid sheet (several per page, sized in mm) rather than one-per-page,
 * matching how a workshop would actually print a batch of stickers to cut out.
 */
export function printQrLabels(labels: PrintableLabel[], settings: PrintSettings = DEFAULT_SETTINGS): void {
  const printWindow = window.open('', '_blank', 'width=480,height=640');
  if (!printWindow) return;

  const { labelSizeMm, columns, paper } = settings;
  const qrSizeMm = Math.round(labelSizeMm * 0.62);

  const labelsHtml = labels
    .map(
      (label) => `
        <div class="label">
          <div class="part-name">${escapeHtml(label.partName.toUpperCase())}</div>
          <img src="${label.qrDataUrl}" alt="QR code" class="qr-image" />
          <div class="bin-code">BIN ${escapeHtml(label.binCode)}</div>
        </div>`,
    )
    .join('');

  printWindow.document.write(`
    <!doctype html>
    <html>
      <head>
        <title>QR Labels</title>
        <style>
          @page { size: ${paper}; margin: 10mm; }
          :root { color-scheme: light; }
          * { box-sizing: border-box; }
          body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            margin: 0;
            background: #fff;
            color: #000;
          }
          .sheet {
            display: grid;
            grid-template-columns: repeat(${columns}, ${labelSizeMm}mm);
            justify-content: center;
            gap: 4mm;
            padding: 6mm;
          }
          .label {
            width: ${labelSizeMm}mm;
            height: ${labelSizeMm}mm;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 2mm;
            background: #fff;
            border: 3px solid #000;
            border-radius: 3mm;
            padding: 3mm;
            text-align: center;
            break-inside: avoid;
            page-break-inside: avoid;
          }
          .part-name {
            font-size: 3.2mm;
            font-weight: 800;
            letter-spacing: 0.03em;
          }
          .qr-image {
            width: ${qrSizeMm}mm;
            height: ${qrSizeMm}mm;
          }
          .bin-code {
            font-size: 3mm;
            font-weight: 700;
            letter-spacing: 0.02em;
          }
        </style>
      </head>
      <body>
        <div class="sheet">
          ${labelsHtml}
        </div>
      </body>
    </html>
  `);
  printWindow.document.close();

  printWindow.onload = () => {
    printWindow.focus();
    printWindow.print();
  };
}

function escapeHtml(value: string): string {
  const div = document.createElement('div');
  div.textContent = value;
  return div.innerHTML;
}
