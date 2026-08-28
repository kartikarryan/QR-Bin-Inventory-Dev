import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-bill-detail',
  imports: [RouterLink],
  templateUrl: './bill-detail.html',
  styleUrl: './bill-detail.scss',
})
export class BillDetail {
  private readonly route = inject(ActivatedRoute);
  readonly billId = Number(this.route.snapshot.paramMap.get('id'));
}
