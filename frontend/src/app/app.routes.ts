import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  {
    path: 'scan/:qrToken',
    loadComponent: () => import('./features/scan/scan').then((m) => m.Scan),
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell').then((m) => m.Shell),
    children: [
      {
        path: 'products',
        loadComponent: () => import('./features/products/products').then((m) => m.Products),
      },
      {
        // The billing counter is the fast path — /bills opens straight into creating a
        // bill rather than a history list, so a user can go from "open the app" to
        // "ringing up a customer" in one navigation.
        path: 'bills',
        loadComponent: () => import('./features/bills/new-bill/new-bill').then((m) => m.NewBill),
      },
      {
        path: 'bills/history',
        loadComponent: () => import('./features/bills/bill-history/bill-history').then((m) => m.BillHistory),
      },
      {
        path: 'bills/:id',
        loadComponent: () => import('./features/bills/bill-detail/bill-detail').then((m) => m.BillDetail),
      },
    ],
  },
  { path: '**', redirectTo: 'products' },
];
