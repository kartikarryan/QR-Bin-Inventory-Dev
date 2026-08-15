import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { InventoryPart } from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  constructor(private readonly http: HttpClient) {}

  getParts(): Observable<ApiResponse<InventoryPart[]>> {
    return this.http.get<ApiResponse<InventoryPart[]>>(`${environment.apiUrl}/inventory`);
  }
}
