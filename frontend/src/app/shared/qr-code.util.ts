import QRCode from 'qrcode';

/** Scan URL encoded into every printed label — resolved by the public `/scan/:qrToken` route. */
export function buildScanUrl(qrToken: string): string {
  return `${window.location.origin}/scan/${qrToken}`;
}

export async function generateQrDataUrl(qrToken: string): Promise<string> {
  // SVG, not raster: labels print at whatever physical mm size the manager picks, on
  // whatever printer DPI they have — a fixed-resolution PNG would go soft at the larger
  // label sizes. 'H' error correction (~30% recoverable) because these live on workshop
  // bins and pick up dust/grease/scuffing. Margin is left at the library default (4
  // modules) — that's the ISO quiet-zone minimum scanners rely on to lock onto the code.
  const svg = await QRCode.toString(buildScanUrl(qrToken), { type: 'svg', errorCorrectionLevel: 'H' });
  return `data:image/svg+xml;utf8,${encodeURIComponent(svg)}`;
}
