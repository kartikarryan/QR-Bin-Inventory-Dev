import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { InventoryPart, StockMovementRequest } from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class ScanService {
  constructor(private readonly http: HttpClient) {}

  getByToken(qrToken: string): Observable<ApiResponse<InventoryPart>> {
    return this.http.get<ApiResponse<InventoryPart>>(`${environment.apiUrl}/scan/${qrToken}`);
  }

  adjustStock(qrToken: string, request: StockMovementRequest): Observable<ApiResponse<InventoryPart>> {
    return this.http.post<ApiResponse<InventoryPart>>(`${environment.apiUrl}/scan/${qrToken}/stock-movements`, request);
  }
}
