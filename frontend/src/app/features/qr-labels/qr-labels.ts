import { Component, computed, inject, signal } from '@angular/core';
import { InventoryService } from '../../core/services/inventory.service';
import { InventoryPart } from '../../core/models/inventory.model';
import { generateQrDataUrl } from '../../shared/qr-code.util';
import { printQrLabels, type PaperSize } from '../../shared/qr-print.util';
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
  readonly printingIds = signal<Set<number>>(new Set());
  readonly bulkPrinting = signal(false);

  readonly previewPart = signal<LabelablePart | null>(null);

  readonly labelSizeMm = signal(60);
  readonly paper = signal<PaperSize>('A4');
  readonly labelsPerSheet = signal(12);

  readonly selectedCount = computed(() => this.selectedIds().size);
  readonly allSelected = computed(() => this.parts().length > 0 && this.selectedIds().size === this.parts().length);

  private readonly columnsForSheet = computed(() => {
    const n = this.labelsPerSheet();
    if (n <= 6) return 2;
    if (n <= 12) return 3;
    return 4;
  });

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

  printOne(part: LabelablePart): void {
    const printing = new Set(this.printingIds());
    printing.add(part.id);
    this.printingIds.set(printing);

    generateQrDataUrl(part.qrToken)
      .then((qrDataUrl) =>
        printQrLabels([{ partName: part.partName, binCode: part.binCode, qrDataUrl }], this.printSettings()),
      )
      .finally(() => {
        const next = new Set(this.printingIds());
        next.delete(part.id);
        this.printingIds.set(next);
      });
  }

  printSelected(): void {
    const selected = this.parts().filter((p) => this.selectedIds().has(p.id));
    if (selected.length === 0) return;

    this.bulkPrinting.set(true);

    Promise.all(
      selected.map(async (part) => ({
        partName: part.partName,
        binCode: part.binCode,
        qrDataUrl: await generateQrDataUrl(part.qrToken),
      })),
    )
      .then((labels) => printQrLabels(labels, this.printSettings()))
      .finally(() => this.bulkPrinting.set(false));
  }

  private printSettings() {
    return {
      labelSizeMm: this.labelSizeMm(),
      columns: this.columnsForSheet(),
      paper: this.paper(),
    };
  }
}
