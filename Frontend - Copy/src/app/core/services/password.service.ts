
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { Users } from '../models/admin.model';

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// export interface RequestResetRequest {
//   email: string;
// }

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}


@Injectable({ providedIn: 'root' })
export class PasswordService {
private user: Users;
  private baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient,private AuthService:AuthService) {}

  changePassword(payload: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/Password/change-password`, payload);
  }

  // requestPasswordReset(email: string): Observable<void> {
  //   return this.http.post<void>(`${this.baseUrl}/Password/{user.id}/password-reset`, {});
  // }






 getMe(): Observable<Users> {
    return this.http.get<Users>(`${this.baseUrl}/me`);
  }


  resetPassword(payload: ResetPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${this.user.id}/reset-password`, {});
  }

  }
  
