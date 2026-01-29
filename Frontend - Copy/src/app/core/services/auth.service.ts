import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse } from '../models/user.model';
import { Users } from '../models/admin.model';
import { VerifyEmailService } from './verify-email.service'

@Injectable({ providedIn: 'root' })
export class AuthService {

  private readonly TOKEN_KEY = 'jwt_token';
  private readonly USERNAME_KEY = 'username';
  private readonly ROLES_KEY = 'roles';

  constructor(
    private http: HttpClient,
    private verifyEmailService: VerifyEmailService   // 👈 injected here
  ) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, {
        username,
        password
      })
      .pipe(tap(res => this.setSession(res)));
  }

  register(username: string, email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/auth/register`, {
      username,
      email,
      password
    });
  }

  // 👇 AuthService calling another service
  verifyEmail(token: string): Observable<void> {
    return this.verifyEmailService.verifyEmail(token);
  }

  private setSession(res: LoginResponse) {
    localStorage.setItem(this.TOKEN_KEY, res.token);
    localStorage.setItem(this.USERNAME_KEY, res.username);
    localStorage.setItem(this.ROLES_KEY, JSON.stringify(res.roles || []));
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USERNAME_KEY);
    localStorage.removeItem(this.ROLES_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getMe(): Observable<Users> {
    return this.http.get<Users>(`${environment.apiUrl}/me`);
  }

  getUsername(): string | null {
    return localStorage.getItem(this.USERNAME_KEY);
  }

  getRoles(): string[] {
    const raw = localStorage.getItem(this.ROLES_KEY);
    return raw ? JSON.parse(raw) : [];
  }

  isAdmin(): boolean {
    return this.getRoles().map(r => r.toLowerCase()).includes('admin');
  }

  isSuperAdmin(): boolean {
    return this.getRoles().map(r => r.toLowerCase()).includes('superadmin');
  }
}
