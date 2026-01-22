import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Comment } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class CommentService {
  constructor(private http: HttpClient) {}

  getComments(postId: string): Observable<Comment[]> {
    return this.http.get<Comment[]>(
      `${environment.apiUrl}/api/Posts/${postId}/comments`
    );
  }

  addComment(postId: string, content: string, parentCommentId?: string | null):Observable<Comment> {
    return this.http.post<Comment>(
      `${environment.apiUrl}/api/Posts/${postId}/comments`,
      {
        content,
        parentCommentId: parentCommentId || null
      }
    );
  }
}
