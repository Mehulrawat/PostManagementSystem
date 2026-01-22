import { Component, OnInit } from '@angular/core';
import { PostService } from '../../core/services/post.service';
import { Post, PostStatus } from '../../core/models/post.model'
import { CommonModule} from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-my-posts',
    standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink], 
  templateUrl: './my-posts.component.html'
})
export class MyPostsComponent implements OnInit {
  posts: Post[] = [];
  isLoading = false;
  errorMessage = '';

  postStatus = PostStatus;

  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.isLoading = true;
    this.errorMessage = '';
    this.postService.getMyPosts().subscribe({
      next: posts => {
        this.posts = posts;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load your posts.';
        this.isLoading = false;
      }
    });
  }

  statusLabel(status: PostStatus): string {
    return PostStatus[status];
  }

  submitForApproval(post: Post) {
    this.postService.submitForApproval(post.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to submit post for approval.')
    });
  }
}
