import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Bill, BillSummary, CreateBillRequest } from '../models/bill.model';

@Injectable({ providedIn: 'root' })
export class BillService {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<ApiResponse<BillSummary[]>> {
    return this.http.get<ApiResponse<BillSummary[]>>(`${environment.apiUrl}/bills`);
  }

  getById(id: number): Observable<ApiResponse<Bill>> {
    return this.http.get<ApiResponse<Bill>>(`${environment.apiUrl}/bills/${id}`);
  }

  create(request: CreateBillRequest): Observable<ApiResponse<Bill>> {
    return this.http.post<ApiResponse<Bill>>(`${environment.apiUrl}/bills`, request);
  }
}
