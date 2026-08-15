import { Component, effect, input, model, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { generateQrDataUrl } from '../qr-code.util';
import { printQrLabels, type PaperSize } from '../qr-print.util';

@Component({
  selector: 'app-qr-label-preview',
  imports: [FormsModule],
  templateUrl: './qr-label-preview.html',
  styleUrl: './qr-label-preview.scss',
})
export class QrLabelPreview {
  readonly partName = input.required<string>();
  readonly binCode = input.required<string>();
  readonly qrToken = input.required<string>();

  /** Opt-in Print Settings panel — Part Details leaves this off; QR Labels turns it on. */
  readonly showSettings = input(false);
  readonly labelSizeMm = model(60);
  readonly paper = model<PaperSize>('A4');
  readonly labelsPerSheet = model(12);

  readonly closed = output<void>();

  readonly qrDataUrl = signal<string | null>(null);

  constructor() {
    effect(() => {
      const token = this.qrToken();
      this.qrDataUrl.set(null);
      generateQrDataUrl(token).then((url) => this.qrDataUrl.set(url));
    });
  }

  print(): void {
    const url = this.qrDataUrl();
    if (!url) return;

    const n = this.labelsPerSheet();
    const columns = n <= 6 ? 2 : n <= 12 ? 3 : 4;

    printQrLabels([{ partName: this.partName(), binCode: this.binCode(), qrDataUrl: url }], {
      labelSizeMm: this.labelSizeMm(),
      columns,
      paper: this.paper(),
    });
  }

  close(): void {
    this.closed.emit();
  }
}
