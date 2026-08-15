import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PartService } from '../../../core/services/part.service';
import { ToastService } from '../../../core/services/toast.service';
import { InventoryPart } from '../../../core/models/inventory.model';
import { QrLabelPreview } from '../../../shared/qr-label-preview/qr-label-preview';

@Component({
  selector: 'app-part-form',
  imports: [ReactiveFormsModule, RouterLink, QrLabelPreview],
  templateUrl: './part-form.html',
  styleUrl: './part-form.scss',
})
export class PartForm {
  private readonly fb = inject(FormBuilder);
  private readonly partService = inject(PartService);
  private readonly toastService = inject(ToastService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly partId = this.route.snapshot.paramMap.get('id')
    ? Number(this.route.snapshot.paramMap.get('id'))
    : null;
  readonly isEditMode = this.partId !== null;

  readonly loading = signal(this.isEditMode);
  readonly loadFailed = signal(false);
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly createdPart = signal<InventoryPart | null>(null);
  readonly showQrPreview = signal(false);

  readonly form = this.fb.nonNullable.group({
    partName: ['', [Validators.required, Validators.maxLength(200)]],
    partNumber: ['', [Validators.maxLength(100)]],
    binCode: ['', [Validators.required, Validators.maxLength(50)]],
    currentStock: [0, [Validators.required, Validators.min(0)]],
    minimumStock: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    if (this.isEditMode && this.partId !== null) {
      this.partService.getById(this.partId).subscribe({
        next: (response) => {
          const part = response.data;
          if (part) {
            this.form.patchValue({
              partName: part.partName,
              partNumber: part.partNumber ?? '',
              binCode: part.binCode ?? '',
              currentStock: part.currentStock,
              minimumStock: part.minimumStock,
            });
          }
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Unable to load this part.');
          this.loadFailed.set(true);
          this.loading.set(false);
        },
      });
    }
  }

  adjustStock(controlName: 'currentStock' | 'minimumStock', delta: number): void {
    const control = this.form.controls[controlName];
    const next = Math.max(0, (control.value ?? 0) + delta);
    control.setValue(next);
    control.markAsTouched();
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const request = {
      partName: raw.partName.trim(),
      partNumber: raw.partNumber.trim() ? raw.partNumber.trim() : null,
      binCode: raw.binCode.trim(),
      currentStock: raw.currentStock,
      minimumStock: raw.minimumStock,
    };

    const save$ =
      this.isEditMode && this.partId !== null
        ? this.partService.update(this.partId, request)
        : this.partService.create(request);

    save$.subscribe({
      next: (response) => {
        this.submitting.set(false);

        if (!this.isEditMode) {
          this.createdPart.set(response.data);
          return;
        }

        this.toastService.show('Part updated', 'success');
        const id = response.data?.id ?? this.partId;
        this.router.navigate(id !== null ? ['/inventory', id] : ['/inventory']);
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.errorMessage.set(error.error?.message ?? 'Unable to save this part.');
      },
    });
  }

  cancel(): void {
    const target = this.isEditMode && this.partId !== null ? ['/inventory', this.partId] : ['/inventory'];
    this.router.navigate(target);
  }
}
