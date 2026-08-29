import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { BillService } from '../../../core/services/bill.service';
import { ToastService } from '../../../core/services/toast.service';
import { Product } from '../../../core/models/product.model';

interface CartLine {
  product: Product;
  quantity: number;
}

@Component({
  selector: 'app-new-bill',
  imports: [FormsModule],
  templateUrl: './new-bill.html',
  styleUrl: './new-bill.scss',
})
export class NewBill {
  private readonly productService = inject(ProductService);
  private readonly billService = inject(BillService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  readonly products = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly searchTerm = signal('');

  readonly cart = signal<Map<number, CartLine>>(new Map());
  readonly customerName = signal('');
  readonly customerPhone = signal('');
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);

  readonly filteredProducts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    return this.products().filter((product) => {
      if (!product.isActive) return false;
      if (!term) return true;
      return product.name.toLowerCase().includes(term) || (product.code?.toLowerCase().includes(term) ?? false);
    });
  });

  readonly cartLines = computed(() => Array.from(this.cart().values()));

  readonly total = computed(() =>
    this.cartLines().reduce((sum, line) => sum + line.product.sellingPrice * line.quantity, 0),
  );

  constructor() {
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

  cartQuantity(productId: number): number {
    return this.cart().get(productId)?.quantity ?? 0;
  }

  addToCart(product: Product): void {
    const nextQty = this.cartQuantity(product.id) + 1;
    if (nextQty > product.currentStock) return;

    const next = new Map(this.cart());
    next.set(product.id, { product, quantity: nextQty });
    this.cart.set(next);
  }

  changeQuantity(productId: number, delta: number): void {
    const line = this.cart().get(productId);
    if (!line) return;

    const clamped = Math.max(1, Math.min(line.quantity + delta, line.product.currentStock));
    const next = new Map(this.cart());
    next.set(productId, { ...line, quantity: clamped });
    this.cart.set(next);
  }

  setQuantity(productId: number, value: number): void {
    const line = this.cart().get(productId);
    if (!line || !Number.isFinite(value)) return;

    const clamped = Math.max(1, Math.min(Math.floor(value), line.product.currentStock));
    const next = new Map(this.cart());
    next.set(productId, { ...line, quantity: clamped });
    this.cart.set(next);
  }

  removeFromCart(productId: number): void {
    const next = new Map(this.cart());
    next.delete(productId);
    this.cart.set(next);
  }

  createBill(): void {
    if (this.cartLines().length === 0 || this.submitting()) return;

    this.submitting.set(true);
    this.submitError.set(null);

    const request = {
      customerName: this.customerName().trim() ? this.customerName().trim() : null,
      customerPhone: this.customerPhone().trim() ? this.customerPhone().trim() : null,
      items: this.cartLines().map((line) => ({ productId: line.product.id, quantity: line.quantity })),
    };

    this.billService.create(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        if (response.data) {
          this.toastService.show('Bill created', 'success');
          this.router.navigate(['/bills', response.data.id]);
        }
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.submitError.set(error.error?.message ?? 'Unable to create this bill.');
      },
    });
  }
}
