import { Component, OnInit } from '@angular/core';
import { PostService } from '../../core/services/post.service';
import { Post } from '../../core/models/post.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommentThreadComponent } from '../../comments/comment-thread/comment-thread.component';
@
Component({
  selector: 'app-post-list',
    standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink,CommentThreadComponent], 
  templateUrl: './post-list.component.html'
})
export class PostListComponent implements OnInit {
  posts: Post[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.load();
  }



  load() {
    this.isLoading = true;
    this.errorMessage = '';
    this.postService.getApprovedPosts().subscribe({
      next: posts => {
        this.posts = posts;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load posts.';
        this.isLoading = false;
      }
    });
  }
}
