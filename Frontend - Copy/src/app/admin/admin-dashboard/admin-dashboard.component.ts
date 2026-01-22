import { Component, OnInit } from '@angular/core';
import { Post, PostStatus } from '../../core/models/post.model';
import { PostService } from '../../core/services/post.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  imports:[CommonModule,RouterLink],
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  posts: Post[] = [];
  isLoading = false;
  errorMessage = '';

  postStatus = PostStatus;


selectedStatus: PostStatus | undefined = undefined;

  // Build options from enum
  statusOptions = [
    { label: 'All', value: undefined },
    { label: 'Draft', value: PostStatus.Draft },
    { label: 'Pending Approval', value: PostStatus.PendingApproval },
    { label: 'Approved', value: PostStatus.Approved },
    { label: 'Resolved', value: PostStatus.Resolved },
    { label: 'Rejected', value: PostStatus.Rejected },
    { label: 'Closed', value: PostStatus.Closed },
  ];




  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.isLoading = true;
    this.postService.getAllPosts().subscribe({
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

onStatusChange(statusStr: string) {
    this.selectedStatus = statusStr === '' ? undefined : Number(statusStr) as PostStatus;
    this.load();
  }

  statusLabel(status: PostStatus) {
    return PostStatus[status];
  }

  approve(post: Post) {
    this.postService.approvePost(post.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to approve post.')
    });
  }

  reject(post: Post) {
    this.postService.rejectPost(post.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to reject post.')
    });
  }

  close(post: Post) {
    this.postService.closePost(post.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to close post.')
    });
  }
}
