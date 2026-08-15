import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'login',
    loadComponent: () => import('./features/login/login').then((m) => m.Login),
    canActivate: [guestGuard],
  },
  {
    path: 'scan/:qrToken',
    loadComponent: () => import('./features/scan/scan').then((m) => m.Scan),
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell').then((m) => m.Shell),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'inventory',
        loadComponent: () => import('./features/inventory/inventory').then((m) => m.Inventory),
      },
      {
        path: 'inventory/add',
        loadComponent: () => import('./features/inventory/part-form/part-form').then((m) => m.PartForm),
      },
      {
        path: 'inventory/:id',
        loadComponent: () => import('./features/inventory/part-details/part-details').then((m) => m.PartDetails),
      },
      {
        path: 'inventory/:id/edit',
        loadComponent: () => import('./features/inventory/part-form/part-form').then((m) => m.PartForm),
      },
      {
        path: 'qr-labels',
        loadComponent: () => import('./features/qr-labels/qr-labels').then((m) => m.QrLabels),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
