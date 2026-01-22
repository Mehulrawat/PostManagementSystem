import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Post, PostCategory, PostStatus } from '../models/post.model';
import { Users } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class SuperAdminService {
  private baseUrl = `${environment.apiUrl}/api/admin/users`;

  constructor(private http: HttpClient) {}

  // registered users and admin
  getAllUsers(): Observable<Users[]> {
    return this.http.get<Users[]>(this.baseUrl);
  }

  suspendUser(userid: string) {
    return this.http.post(`${this.baseUrl}/${userid}/suspend`,{});
  }
 reinstateUser(userid: string) {
    return this.http.post(`${this.baseUrl}/${userid}/reinstate`,{});
  }
  deleteuser(userid: string) {
    return this.http.delete(`${this.baseUrl}/${userid}`, {});
  }

 promoteUser(userid: string) {
    return this.http.post(`${this.baseUrl}/${userid}/promote`,{});
  }

   demoteUser(userid: string) {
    return this.http.post(`${this.baseUrl}/${userid}/demote`,{});
  }

}
