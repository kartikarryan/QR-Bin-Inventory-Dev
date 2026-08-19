import { Component, computed, inject, signal } from '@angular/core';
import { InventoryService } from '../../core/services/inventory.service';
import { InventoryPart } from '../../core/models/inventory.model';
import { generateQrPngDataUrl } from '../../shared/qr-code.util';
import { downloadQrLabelsPdf } from '../../shared/qr-print.util';
import { QrLabelPreview } from '../../shared/qr-label-preview/qr-label-preview';

interface LabelablePart extends InventoryPart {
  binCode: string;
  qrToken: string;
}

@Component({
  selector: 'app-qr-labels',
  imports: [QrLabelPreview],
  templateUrl: './qr-labels.html',
  styleUrl: './qr-labels.scss',
})
export class QrLabels {
  private readonly inventoryService = inject(InventoryService);

  readonly parts = signal<LabelablePart[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly selectedIds = signal<Set<number>>(new Set());
  readonly downloadingIds = signal<Set<number>>(new Set());
  readonly bulkDownloading = signal(false);

  readonly previewPart = signal<LabelablePart | null>(null);

  readonly selectedCount = computed(() => this.selectedIds().size);
  readonly allSelected = computed(() => this.parts().length > 0 && this.selectedIds().size === this.parts().length);

  constructor() {
    this.loadParts();
  }

  loadParts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.inventoryService.getParts().subscribe({
      next: (response) => {
        const labelable = (response.data ?? []).filter(
          (part): part is LabelablePart => !!part.binCode && !!part.qrToken,
        );
        this.parts.set(labelable);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load bins.');
        this.loading.set(false);
      },
    });
  }

  isSelected(id: number): boolean {
    return this.selectedIds().has(id);
  }

  toggleSelected(id: number): void {
    const next = new Set(this.selectedIds());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.selectedIds.set(next);
  }

  toggleSelectAll(): void {
    this.selectedIds.set(this.allSelected() ? new Set() : new Set(this.parts().map((p) => p.id)));
  }

  viewQr(part: LabelablePart): void {
    this.previewPart.set(part);
  }

  closePreview(): void {
    this.previewPart.set(null);
  }

  downloadOne(part: LabelablePart): void {
    const downloading = new Set(this.downloadingIds());
    downloading.add(part.id);
    this.downloadingIds.set(downloading);

    generateQrPngDataUrl(part.qrToken)
      .then((qrDataUrl) =>
        downloadQrLabelsPdf(
          [{ partName: part.partName, binCode: part.binCode, qrDataUrl }],
          `qr-label-${part.binCode}.pdf`,
        ),
      )
      .finally(() => {
        const next = new Set(this.downloadingIds());
        next.delete(part.id);
        this.downloadingIds.set(next);
      });
  }

  downloadSelected(): void {
    const selected = this.parts().filter((p) => this.selectedIds().has(p.id));
    if (selected.length === 0) return;

    this.bulkDownloading.set(true);

    Promise.all(
      selected.map(async (part) => ({
        partName: part.partName,
        binCode: part.binCode,
        qrDataUrl: await generateQrPngDataUrl(part.qrToken),
      })),
    )
      .then((labels) => downloadQrLabelsPdf(labels, 'qr-labels.pdf'))
      .finally(() => this.bulkDownloading.set(false));
  }
}
