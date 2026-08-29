import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, effect, inject, input, OnDestroy, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { GST_RATES, PRODUCT_UNITS } from '../../../core/models/product.model';
import { lockBodyScroll } from '../../../shared/scroll-lock.util';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductForm implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly toastService = inject(ToastService);

  /** null = Add Product, a product id = Edit Product. */
  readonly productId = input<number | null>(null);
  readonly closed = output<void>();
  readonly saved = output<void>();

  readonly units = PRODUCT_UNITS;
  readonly gstRates = GST_RATES;
  readonly isEditMode = computed(() => this.productId() !== null);

  readonly loading = signal(false);
  readonly loadFailed = signal(false);
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  /** Display-only in edit mode — current stock is never editable from this form. */
  readonly currentStock = signal<number | null>(null);
  readonly currentUnit = signal<string>('pcs');

  private readonly unlockScroll = lockBodyScroll();

  readonly form = this.fb.group({
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(200)]),
    code: this.fb.nonNullable.control('', [Validators.maxLength(100)]),
    hsnCode: this.fb.nonNullable.control('', [Validators.maxLength(20)]),
    unit: this.fb.nonNullable.control('pcs', [Validators.required]),
    // Blank, not 0 — a prefilled zero forces the user to delete it before typing a real number.
    sellingPrice: this.fb.control<number | null>(null, [Validators.required, Validators.min(0)]),
    gstRate: this.fb.nonNullable.control(18, [Validators.required]),
    openingStock: this.fb.control<number | null>(null, [Validators.required, Validators.min(0)]),
  });

  constructor() {
    effect(() => {
      const id = this.productId();

      this.form.reset({ name: '', code: '', hsnCode: '', unit: 'pcs', sellingPrice: null, gstRate: 18, openingStock: null });
      this.errorMessage.set(null);
      this.loadFailed.set(false);
      this.currentStock.set(null);

      if (id === null) {
        this.form.controls.openingStock.enable();
        this.loading.set(false);
        return;
      }

      // Opening Stock only applies at creation — not part of the edit form at all.
      this.form.controls.openingStock.disable();
      this.loading.set(true);

      this.productService.getById(id).subscribe({
        next: (response) => {
          const product = response.data;
          if (product) {
            this.form.patchValue({
              name: product.name,
              code: product.code ?? '',
              hsnCode: product.hsnCode ?? '',
              unit: product.unit,
              sellingPrice: product.sellingPrice,
              gstRate: product.gstRate,
            });
            this.currentStock.set(product.currentStock);
            this.currentUnit.set(product.unit);
          }
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Unable to load this product.');
          this.loadFailed.set(true);
          this.loading.set(false);
        },
      });
    });
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const code = raw.code.trim() ? raw.code.trim() : null;
    const hsnCode = raw.hsnCode.trim() ? raw.hsnCode.trim() : null;
    const sellingPrice = raw.sellingPrice ?? 0;
    const openingStock = raw.openingStock ?? 0;
    const id = this.productId();

    const save$ =
      id !== null
        ? this.productService.update(id, {
            name: raw.name.trim(),
            code,
            hsnCode,
            unit: raw.unit,
            sellingPrice,
            gstRate: raw.gstRate,
          })
        : this.productService.create({
            name: raw.name.trim(),
            code,
            hsnCode,
            unit: raw.unit,
            sellingPrice,
            gstRate: raw.gstRate,
            openingStock,
          });

    save$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.toastService.show(this.isEditMode() ? 'Product updated' : 'Product added successfully', 'success');
        this.saved.emit();
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.errorMessage.set(error.error?.message ?? 'Unable to save this product.');
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
