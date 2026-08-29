import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';
import { ToastService } from '../../core/services/toast.service';
import { Product } from '../../core/models/product.model';
import { ProductForm } from './product-form/product-form';
import { AddStockDialog } from './add-stock-dialog/add-stock-dialog';

/** Below this quantity a product reads as "low stock" (amber) rather than healthy (green).
 *  There's no per-product minimum-stock field in this data model yet — this is a flat
 *  heuristic, not a configurable threshold. */
const LOW_STOCK_THRESHOLD = 10;

@Component({
  selector: 'app-products',
  imports: [FormsModule, ProductForm, AddStockDialog],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class Products {
  private readonly productService = inject(ProductService);
  private readonly toastService = inject(ToastService);

  readonly products = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly searchTerm = signal('');
  readonly togglingIds = signal<Set<number>>(new Set());
  readonly activeFilter = signal<'active' | 'inactive' | 'out-of-stock'>('active');

  /** Whether the Add/Edit sheet is open, and which product it's editing (null = Add). */
  readonly showForm = signal(false);
  readonly editingProductId = signal<number | null>(null);

  /** Which product the Add Stock dialog is open for, if any. */
  readonly addStockFor = signal<Product | null>(null);

  readonly activeCount = computed(() => this.products().filter((p) => p.isActive && p.currentStock > 0).length);
  readonly inactiveCount = computed(() => this.products().filter((p) => !p.isActive).length);
  readonly outOfStockCount = computed(() => this.products().filter((p) => p.isActive && p.currentStock <= 0).length);

  readonly filteredProducts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const filter = this.activeFilter();

    return this.products().filter((product) => {
      const matchesFilter =
        filter === 'inactive'
          ? !product.isActive
          : filter === 'out-of-stock'
            ? product.isActive && product.currentStock <= 0
            : product.isActive && product.currentStock > 0;

      if (!matchesFilter) return false;
      if (!term) return true;
      return product.name.toLowerCase().includes(term) || (product.code?.toLowerCase().includes(term) ?? false);
    });
  });

  constructor() {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.productService.getAll().subscribe({
      next: (response) => {
        this.products.set(response.data ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load products.');
        this.loading.set(false);
      },
    });
  }

  stockClass(stock: number): string {
    if (stock <= 0) return 'danger';
    if (stock <= LOW_STOCK_THRESHOLD) return 'warning';
    return '';
  }

  openAddForm(): void {
    this.editingProductId.set(null);
    this.showForm.set(true);
  }

  openEditForm(product: Product): void {
    this.editingProductId.set(product.id);
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
  }

  onFormSaved(): void {
    this.showForm.set(false);
    this.loadProducts();
  }

  openAddStock(product: Product): void {
    this.addStockFor.set(product);
  }

  closeAddStock(): void {
    this.addStockFor.set(null);
  }

  onStockSaved(): void {
    this.addStockFor.set(null);
    this.loadProducts();
  }

  toggleActive(product: Product): void {
    const toggling = new Set(this.togglingIds());
    toggling.add(product.id);
    this.togglingIds.set(toggling);

    const nextActive = !product.isActive;

    this.productService.setActive(product.id, { isActive: nextActive }).subscribe({
      next: (response) => {
        if (response.data) {
          this.products.update((list) => list.map((p) => (p.id === product.id ? response.data! : p)));
          this.toastService.show(nextActive ? 'Product activated' : 'Product deactivated', 'success');
        }
        const next = new Set(this.togglingIds());
        next.delete(product.id);
        this.togglingIds.set(next);
      },
      error: () => {
        this.toastService.show('Unable to update this product.', 'error');
        const next = new Set(this.togglingIds());
        next.delete(product.id);
        this.togglingIds.set(next);
      },
    });
  }
}
