import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ScanService } from '../../core/services/scan.service';
import { ToastService } from '../../core/services/toast.service';
import { InventoryPart, InventoryStatus } from '../../core/models/inventory.model';

@Component({
  selector: 'app-scan',
  imports: [FormsModule],
  templateUrl: './scan.html',
  styleUrl: './scan.scss',
})
export class Scan {
  private readonly route = inject(ActivatedRoute);
  private readonly scanService = inject(ScanService);
  private readonly toastService = inject(ToastService);

  readonly qrToken = this.route.snapshot.paramMap.get('qrToken') ?? '';

  readonly part = signal<InventoryPart | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly adjustAmount = signal(1);
  readonly adjusting = signal(false);
  readonly adjustError = signal<string | null>(null);

  constructor() {
    this.loadPart();
  }

  loadPart(): void {
    this.scanService.getByToken(this.qrToken).subscribe({
      next: (response) => {
        this.part.set(response.data);
        this.loading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(error.error?.message ?? 'Unable to load this bin.');
        this.loading.set(false);
      },
    });
  }

  statusClass(status: InventoryStatus): string {
    switch (status) {
      case 'In Stock':
        return 'in-stock';
      case 'Low Stock':
        return 'low-stock';
      case 'Out of Stock':
        return 'out-of-stock';
    }
  }

  /** Fraction of the way from empty to a "healthy" level (2x minimum stock), clamped to 0-100. */
  stockBarPercent(currentStock: number, minimumStock: number): number {
    const healthyLevel = minimumStock > 0 ? minimumStock * 2 : 1;
    return Math.max(0, Math.min(100, Math.round((currentStock / healthyLevel) * 100)));
  }

  adjustStock(direction: 1 | -1): void {
    const current = this.part();
    const amount = Math.trunc(this.adjustAmount());
    if (!current || this.adjusting() || !Number.isFinite(amount) || amount <= 0) return;

    const quantityDelta = direction * amount;
    if (direction === -1 && amount > current.currentStock) {
      this.adjustError.set(`Only ${current.currentStock} in stock — can't remove ${amount}.`);
      return;
    }

    this.adjusting.set(true);
    this.adjustError.set(null);
    const previousStock = current.currentStock;

    this.scanService.adjustStock(this.qrToken, { quantityDelta }).subscribe({
      next: (response) => {
        this.adjusting.set(false);
        if (response.data) {
          this.part.set(response.data);
          this.toastService.show(
            `Stock updated — ${response.data.partName}: ${previousStock} → ${response.data.currentStock}`,
            'success',
          );
        }
      },
      error: (error: HttpErrorResponse) => {
        this.adjusting.set(false);
        this.adjustError.set(error.error?.message ?? 'Unable to update stock.');
      },
    });
  }
}
