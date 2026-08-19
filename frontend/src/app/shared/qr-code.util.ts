import QRCode from 'qrcode';

/** Scan URL encoded into every printed label — resolved by the public `/scan/:qrToken` route. */
export function buildScanUrl(qrToken: string): string {
  return `${window.location.origin}/scan/${qrToken}`;
}

export async function generateQrDataUrl(qrToken: string): Promise<string> {
  // SVG, not raster: for on-screen preview, resolution-independent regardless of display
  // density. 'H' error correction (~30% recoverable) because these live on workshop bins
  // and pick up dust/grease/scuffing. Margin is left at the library default (4 modules) —
  // that's the ISO quiet-zone minimum scanners rely on to lock onto the code.
  const svg = await QRCode.toString(buildScanUrl(qrToken), { type: 'svg', errorCorrectionLevel: 'H' });
  return `data:image/svg+xml;utf8,${encodeURIComponent(svg)}`;
}

/** Raster PNG for embedding into the downloadable PDF — jsPDF's addImage() needs a bitmap, not SVG. */
export function generateQrPngDataUrl(qrToken: string): Promise<string> {
  return QRCode.toDataURL(buildScanUrl(qrToken), { errorCorrectionLevel: 'H', width: 600 });
}
