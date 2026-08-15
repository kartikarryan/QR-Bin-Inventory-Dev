import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ManagerLoginRequest, ManagerLoginResponse, ManagerProfile } from '../models/auth.model';
import { ApiResponse } from '../models/api-response.model';

const TOKEN_KEY = 'qrbin.token';
const EXPIRES_KEY = 'qrbin.tokenExpiresAt';
const MANAGER_KEY = 'qrbin.manager';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly manager = signal<ManagerProfile | null>(this.readStoredManager());
  private readonly token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly expiresAt = signal<string | null>(localStorage.getItem(EXPIRES_KEY));

  readonly currentManager = this.manager.asReadonly();
  readonly isAuthenticated = computed(() => {
    const token = this.token();
    const expiresAt = this.expiresAt();
    if (!token || !expiresAt) return false;
    return new Date(expiresAt).getTime() > Date.now();
  });

  constructor(private readonly http: HttpClient) {}

  login(request: ManagerLoginRequest): Observable<ApiResponse<ManagerLoginResponse>> {
    return this.http
      .post<ApiResponse<ManagerLoginResponse>>(`${environment.apiUrl}/auth/manager/login`, request)
      .pipe(
        tap((response) => {
          if (response.data) {
            this.setSession(response.data);
          }
        }),
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_KEY);
    localStorage.removeItem(MANAGER_KEY);
    this.token.set(null);
    this.expiresAt.set(null);
    this.manager.set(null);
  }

  getToken(): string | null {
    return this.token();
  }

  private setSession(data: ManagerLoginResponse): void {
    localStorage.setItem(TOKEN_KEY, data.token);
    localStorage.setItem(EXPIRES_KEY, data.expiresAt);
    localStorage.setItem(MANAGER_KEY, JSON.stringify(data.manager));
    this.token.set(data.token);
    this.expiresAt.set(data.expiresAt);
    this.manager.set(data.manager);
  }

  private readStoredManager(): ManagerProfile | null {
    const raw = localStorage.getItem(MANAGER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as ManagerProfile;
    } catch {
      return null;
    }
  }
}
