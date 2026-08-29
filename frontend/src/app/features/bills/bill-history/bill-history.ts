import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BillService } from '../../../core/services/bill.service';
import { BillSummary } from '../../../core/models/bill.model';

@Component({
  selector: 'app-bill-history',
  imports: [RouterLink, DatePipe],
  templateUrl: './bill-history.html',
  styleUrl: './bill-history.scss',
})
export class BillHistory {
  private readonly billService = inject(BillService);

  readonly bills = signal<BillSummary[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.billService.getAll().subscribe({
      next: (response) => {
        this.bills.set(response.data ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load bills.');
        this.loading.set(false);
      },
    });
  }
}
