import { Component, effect, input, output, signal } from '@angular/core';
import { generateQrDataUrl, generateQrPngDataUrl } from '../qr-code.util';
import { downloadQrLabelsPdf } from '../qr-print.util';

@Component({
  selector: 'app-qr-label-preview',
  templateUrl: './qr-label-preview.html',
  styleUrl: './qr-label-preview.scss',
})
export class QrLabelPreview {
  readonly partName = input.required<string>();
  readonly binCode = input.required<string>();
  readonly qrToken = input.required<string>();

  readonly closed = output<void>();

  readonly qrDataUrl = signal<string | null>(null);
  readonly downloading = signal(false);

  constructor() {
    effect(() => {
      const token = this.qrToken();
      this.qrDataUrl.set(null);
      generateQrDataUrl(token).then((url) => this.qrDataUrl.set(url));
    });
  }

  async download(): Promise<void> {
    if (this.downloading()) return;
    this.downloading.set(true);
    try {
      const qrDataUrl = await generateQrPngDataUrl(this.qrToken());
      downloadQrLabelsPdf(
        [{ partName: this.partName(), binCode: this.binCode(), qrDataUrl }],
        `qr-label-${this.binCode()}.pdf`,
      );
    } finally {
      this.downloading.set(false);
    }
  }

  close(): void {
    this.closed.emit();
  }
}
