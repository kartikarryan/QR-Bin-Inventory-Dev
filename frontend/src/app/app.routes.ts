import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'billing' },
  {
    path: 'scan/:qrToken',
    loadComponent: () => import('./features/scan/scan').then((m) => m.Scan),
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell').then((m) => m.Shell),
    children: [
      {
        path: 'billing',
        loadComponent: () => import('./features/billing/billing').then((m) => m.Billing),
      },
      {
        path: 'stock',
        loadComponent: () => import('./features/stock/stock').then((m) => m.Stock),
      },
      {
        path: 'products',
        loadComponent: () => import('./features/products/products').then((m) => m.Products),
      },
      {
        path: 'bills',
        loadComponent: () => import('./features/bills/bill-history/bill-history').then((m) => m.BillHistory),
      },
      {
        path: 'bills/:id',
        loadComponent: () => import('./features/bills/bill-detail/bill-detail').then((m) => m.BillDetail),
      },
    ],
  },
  { path: '**', redirectTo: 'billing' },
];
