import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { DashboardService } from '../../core/services/dashboard.service';
import { InventoryService } from '../../core/services/inventory.service';
import { DashboardSummary } from '../../core/models/dashboard.model';
import { InventoryPart, InventoryStatus } from '../../core/models/inventory.model';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly dashboardService = inject(DashboardService);
  private readonly inventoryService = inject(InventoryService);

  readonly summary = signal<DashboardSummary | null>(null);
  readonly needsAttention = signal<InventoryPart[]>([]);
  readonly loading = signal(true);
  readonly refreshing = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly lastUpdated = signal<Date | null>(null);

  constructor() {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.fetch(() => this.loading.set(false));
  }

  /** Re-fetches in place — never touches `loading`, so the page never flashes back to skeletons. */
  refresh(): void {
    if (this.loading() || this.refreshing()) return;
    this.refreshing.set(true);
    this.fetch(() => this.refreshing.set(false));
  }

  private fetch(onSettled: () => void): void {
    // The dashboard summary's lowStockParts only ever contains Low Stock rows (Out of Stock is
    // just a count there) — pulling the full inventory list too lets this table show both
    // together, matching the design, without any backend/API change.
    forkJoin({
      summary: this.dashboardService.getSummary(),
      inventory: this.inventoryService.getParts(),
    }).subscribe({
      next: ({ summary, inventory }) => {
        this.summary.set(summary.data);

        const attention = (inventory.data ?? [])
          .filter((part) => part.status !== 'In Stock')
          .sort((a, b) => a.currentStock - b.currentStock)
          .slice(0, 10);
        this.needsAttention.set(attention);
        this.lastUpdated.set(new Date());

        onSettled();
      },
      error: () => {
        this.errorMessage.set('Unable to load dashboard data.');
        onSettled();
      },
    });
  }

  formattedLastUpdated(): string {
    const date = this.lastUpdated();
    return date ? date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : '';
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
}
