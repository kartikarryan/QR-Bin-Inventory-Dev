import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { InventoryService } from '../../core/services/inventory.service';
import { InventoryPart } from '../../core/models/inventory.model';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-inventory',
  imports: [FormsModule, RouterLink],
  templateUrl: './inventory.html',
  styleUrl: './inventory.scss',
})
export class Inventory {
  private readonly inventoryService = inject(InventoryService);

  readonly parts = signal<InventoryPart[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly searchTerm = signal('');
  readonly currentPage = signal(1);

  readonly filteredParts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.parts();

    return this.parts().filter(
      (part) =>
        part.partName.toLowerCase().includes(term) ||
        (part.partNumber?.toLowerCase().includes(term) ?? false) ||
        (part.binCode?.toLowerCase().includes(term) ?? false),
    );
  });

  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredParts().length / PAGE_SIZE)));

  readonly pagedParts = computed(() => {
    const start = (this.currentPage() - 1) * PAGE_SIZE;
    return this.filteredParts().slice(start, start + PAGE_SIZE);
  });

  readonly pageNumbers = computed<(number | 'ellipsis')[]>(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);

    const pages: (number | 'ellipsis')[] = [1];
    if (current > 3) pages.push('ellipsis');
    for (let p = Math.max(2, current - 1); p <= Math.min(total - 1, current + 1); p++) pages.push(p);
    if (current < total - 2) pages.push('ellipsis');
    pages.push(total);
    return pages;
  });

  constructor() {
    this.loadParts();
  }

  loadParts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.inventoryService.getParts().subscribe({
      next: (response) => {
        this.parts.set(response.data ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load inventory.');
        this.loading.set(false);
      },
    });
  }

  onSearchTermChange(value: string): void {
    this.searchTerm.set(value);
    this.currentPage.set(1);
  }

  goToPage(page: number): void {
    this.currentPage.set(Math.min(Math.max(1, page), this.totalPages()));
  }

  statusClass(status: InventoryPart['status']): string {
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
