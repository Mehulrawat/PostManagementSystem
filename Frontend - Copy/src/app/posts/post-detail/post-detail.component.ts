
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PostService } from '../../core/services/post.service';
import { Post } from '../../core/models/post.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { CommentThreadComponent } from 'src/app/comments/comment-thread/comment-thread.component';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, CommentThreadComponent],
  templateUrl: './post-detail.component.html'
})
export class PostDetailComponent implements OnInit {
  post: Post | null = null;
  isLoading = false;
  errorMessage = '';
  expandedPostId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private postService: PostService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.load(id);
  }

  load(id: string) {
    this.isLoading = true;
    this.postService.getPost(id).subscribe({
      next: p => {
        this.post = p;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load post.';
        this.isLoading = false;
      }
    });
  }

  toggleComments(postId: string) {
    this.expandedPostId = this.expandedPostId === postId ? null : postId;
  }
}
