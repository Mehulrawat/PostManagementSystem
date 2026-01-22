import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Post, PostCategory, PostStatus } from '../models/post.model';
import { HttpParams } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class PostService {
  private baseUrl = `${environment.apiUrl}/api/Posts`;

  constructor(private http: HttpClient) {}

  // public approved posts
  getApprovedPosts(): Observable<Post[]> {
    return this.http.get<Post[]>(this.baseUrl);
  }

  // admin view
  getAllPosts(status?: PostStatus): Observable<Post[]> {
    
  const params = (status !== undefined)
    ? new HttpParams().set('status', String(status)) // "2" for Approved
    : undefined;

    return this.http.get<Post[]>(`${this.baseUrl}/admin`, {params});
  }

  getMyPosts(): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.baseUrl}/my`);
  }

  getPost(id: string): Observable<Post> {
    return this.http.get<Post>(`${this.baseUrl}/${id}`);
  }

  createPost(title: string, content: string, category: PostCategory) {
    return this.http.post<Post>(this.baseUrl, { title, content, category });
  }

  submitForApproval(id: string) {
    return this.http.post(`${this.baseUrl}/${id}/submit`, {});
  }

  // Admin actions
  approvePost(id: string) {
    return this.http.post(`${this.baseUrl}/${id}/approve`, {});
  }

  rejectPost(id: string) {
    return this.http.post(`${this.baseUrl}/${id}/reject`, {});
  }

  closePost(id: string) {
    return this.http.post(`${this.baseUrl}/${id}/close`, {});
  }

  resolvePost(id: string) {
    return this.http.post(`${this.baseUrl}/${id}/resolve`, {});
  }

  assignPost(id: string, assigneeId: string) {
    return this.http.post(`${this.baseUrl}/${id}/assign/${assigneeId}`, {});
  }
}
