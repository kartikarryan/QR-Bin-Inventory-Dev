import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { CreatePartRequest, InventoryPart, StockMovementRequest, UpdatePartRequest } from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class PartService {
  constructor(private readonly http: HttpClient) {}

  getById(id: number): Observable<ApiResponse<InventoryPart>> {
    return this.http.get<ApiResponse<InventoryPart>>(`${environment.apiUrl}/parts/${id}`);
  }

  create(request: CreatePartRequest): Observable<ApiResponse<InventoryPart>> {
    return this.http.post<ApiResponse<InventoryPart>>(`${environment.apiUrl}/parts`, request);
  }

  update(id: number, request: UpdatePartRequest): Observable<ApiResponse<InventoryPart>> {
    return this.http.put<ApiResponse<InventoryPart>>(`${environment.apiUrl}/parts/${id}`, request);
  }

  adjustStock(id: number, request: StockMovementRequest): Observable<ApiResponse<InventoryPart>> {
    return this.http.post<ApiResponse<InventoryPart>>(`${environment.apiUrl}/parts/${id}/stock-movements`, request);
  }
}
