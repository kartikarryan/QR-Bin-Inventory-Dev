import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, OnDestroy, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { Product } from '../../../core/models/product.model';
import { lockBodyScroll } from '../../../shared/scroll-lock.util';

@Component({
  selector: 'app-add-stock-dialog',
  imports: [FormsModule],
  templateUrl: './add-stock-dialog.html',
  styleUrl: './add-stock-dialog.scss',
})
export class AddStockDialog implements OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly toastService = inject(ToastService);

  readonly product = input.required<Product>();
  readonly closed = output<void>();
  readonly saved = output<void>();

  readonly quantity = signal<number | null>(null);
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  private readonly unlockScroll = lockBodyScroll();

  readonly newStock = computed(() => {
    const qty = this.quantity();
    return qty && qty > 0 ? this.product().currentStock + qty : null;
  });

  submit(): void {
    const qty = this.quantity();
    if (!qty || qty <= 0 || this.submitting()) return;

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.productService.addStock(this.product().id, { quantity: qty }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.toastService.show('Stock updated successfully', 'success');
        this.saved.emit();
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.errorMessage.set(error.error?.message ?? 'Unable to update stock.');
      },
    });
  }

  close(): void {
    this.closed.emit();
  }

  ngOnDestroy(): void {
    this.unlockScroll();
  }
}
