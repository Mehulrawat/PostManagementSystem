import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PostService } from '../../core/services/post.service';
import { PostCategory } from '../../core/models/post.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-post-create',
    standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink], 
  templateUrl: './post-create.component.html'
})
export class PostCreateComponent {
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  postCategory = PostCategory;
  categoryKeys = Object.keys(PostCategory).filter(k => !isNaN(Number(PostCategory[k as any])));

  form = this.fb.group({
    title: ['', Validators.required],
    content: ['', Validators.required],
    category: [PostCategory.Issue, Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private postService: PostService,
    private router: Router
  ) {}

  submit() {
    if (this.form.invalid) return;

    const { title, content, category } = this.form.value;
    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.postService
      .createPost(title!, content!, Number(category))
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.successMessage = 'Post created successfully.';
          setTimeout(() => this.router.navigate(['/posts']), 800);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = 'Failed to create post.';
        }
      });
  }
}
