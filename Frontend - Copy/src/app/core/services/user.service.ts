import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Users } from '../models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl =`${environment.apiUrl}/api/User/me`;

  constructor(private http: HttpClient) { }

  // // Get all users
  // getUsers(): Observable<Users[]> {
  //   return this.http.get<Users[]>(this.baseUrl);
  // }

  // Get user by ID
  getUserById(id: string): Observable<Users> {
    return this.http.get<Users>(`${this.baseUrl}/${id}`);
  }
}
