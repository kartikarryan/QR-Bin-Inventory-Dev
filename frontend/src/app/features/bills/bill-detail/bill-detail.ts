import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BillService } from '../../../core/services/bill.service';
import { Bill, BillItem } from '../../../core/models/bill.model';
import { BillReturnDialog } from '../bill-return-dialog/bill-return-dialog';

@Component({
  selector: 'app-bill-detail',
  imports: [RouterLink, DatePipe, BillReturnDialog],
  templateUrl: './bill-detail.html',
  styleUrl: './bill-detail.scss',
})
export class BillDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly billService = inject(BillService);

  readonly billId = Number(this.route.snapshot.paramMap.get('id'));

  readonly bill = signal<Bill | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  /** The line item currently being returned/exchanged, if the dialog is open. */
  readonly returningItem = signal<BillItem | null>(null);

  constructor() {
    this.billService.getById(this.billId).subscribe({
      next: (response) => {
        this.bill.set(response.data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load this bill.');
        this.loading.set(false);
      },
    });
  }

  print(): void {
    window.print();
  }

  openReturn(item: BillItem): void {
    this.returningItem.set(item);
  }

  closeReturn(): void {
    this.returningItem.set(null);
  }

  onReturnSaved(updated: Bill): void {
    this.bill.set(updated);
    this.returningItem.set(null);
  }
}
