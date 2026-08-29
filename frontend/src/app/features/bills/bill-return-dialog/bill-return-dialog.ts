import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, OnDestroy, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BillService } from '../../../core/services/bill.service';
import { ProductService } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { Bill, BillItem } from '../../../core/models/bill.model';
import { Product } from '../../../core/models/product.model';
import { lockBodyScroll } from '../../../shared/scroll-lock.util';

@Component({
  selector: 'app-bill-return-dialog',
  imports: [FormsModule],
  templateUrl: './bill-return-dialog.html',
  styleUrl: './bill-return-dialog.scss',
})
export class BillReturnDialog implements OnDestroy {
  private readonly billService = inject(BillService);
  private readonly productService = inject(ProductService);
  private readonly toastService = inject(ToastService);

  readonly bill = input.required<Bill>();
  readonly item = input.required<BillItem>();
  readonly closed = output<void>();
  readonly saved = output<Bill>();

  readonly remaining = computed(() => this.item().quantity - this.item().returnedQuantity);

  readonly returnQuantity = signal<number | null>(null);
  readonly isExchange = signal(false);
  readonly replacementProductId = signal<number | null>(null);
  readonly replacementQuantity = signal<number | null>(null);
  readonly notes = signal('');
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  private readonly products = signal<Product[]>([]);
  private readonly unlockScroll = lockBodyScroll();

  readonly replacementOptions = computed(() =>
    this.products().filter((p) => p.isActive && p.id !== this.item().productId),
  );

  readonly effectiveReturnQuantity = computed(() => this.returnQuantity() ?? this.remaining());

  constructor() {
    this.productService.getAll().subscribe({
      next: (response) => this.products.set(response.data ?? []),
    });
  }

  submit(): void {
    if (this.submitting()) return;

    const qty = this.effectiveReturnQuantity();
    if (!qty || qty < 1 || qty > this.remaining()) {
      this.errorMessage.set(`Enter a quantity between 1 and ${this.remaining()}.`);
      return;
    }

    if (this.isExchange() && (!this.replacementProductId() || !this.replacementQuantity() || this.replacementQuantity()! < 1)) {
      this.errorMessage.set('Choose a replacement product and quantity.');
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.billService
      .createReturn(this.bill().id, {
        billItemId: this.item().id,
        returnedQuantity: qty,
        replacementProductId: this.isExchange() ? this.replacementProductId() : null,
        replacementQuantity: this.isExchange() ? this.replacementQuantity() : null,
        notes: this.notes().trim() ? this.notes().trim() : null,
      })
      .subscribe({
        next: (response) => {
          this.submitting.set(false);
          if (response.data) {
            this.toastService.show('Return recorded', 'success');
            this.saved.emit(response.data);
          }
        },
        error: (error: HttpErrorResponse) => {
          this.submitting.set(false);
          this.errorMessage.set(error.error?.message ?? 'Unable to record this return.');
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
