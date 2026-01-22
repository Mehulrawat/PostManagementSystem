
import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Comment } from '../../core/models/comment.model';
import { CommentService } from '../../core/services/comment.service';
import { AuthService } from '../../core/services/auth.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-comment-thread',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,RouterLink],
  templateUrl: './comment-thread.component.html'
})
export class CommentThreadComponent implements OnInit {
  @Input() postId!: string;
  @Input() comments: Comment[] = []; // ✅ For recursive rendering

  isLoading = false;
  errorMessage = '';
  isSubmitting = false;
 @Input() isApproved!: boolean;
  form = this.fb.group({
    content: ['', Validators.required]
  });

  replyingTo: string | null = null;

  constructor(
    private commentService: CommentService,
    public auth: AuthService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    //  Load only if top-level comments
    if (!this.comments.length && this.postId) {
      this.load();
    }
  }

  load() {
    this.isLoading = true;
    this.commentService.getComments(this.postId).subscribe({
      next: comments => {
        this.comments = comments;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load comments.';
        this.isLoading = false;
      }
    });
  }

 startReply(commentId: string) {
   
    if (!this.isApproved) return;
    this.replyingTo = commentId;
  }

  cancelReply() {
    this.replyingTo = null;
  }

  submit() {
    if (this.form.invalid || !this.auth.isLoggedIn()) return;

    this.isSubmitting = true;
    const content = this.form.value.content!;
    this.commentService.addComment(this.postId, content, this.replyingTo).subscribe({
      next: (newComment) => {
        this.isSubmitting = false;
        this.form.reset();
        this.replyingTo = null;


        if (this.replyingTo) {
          const parent = this.findComment(this.comments, this.replyingTo);
          if (parent) {
            parent.replies = parent.replies || [];
            parent.replies.push(newComment);
          }
        } else {
          this.comments.push(newComment);
        }
      },
      error: () => {
        this.errorMessage = 'Failed to add comment.';
        this.isSubmitting = false;
      }
    });


        
    //     if (this.replyingTo) {
    //       const parent = this.findComment(this.comments, this.replyingTo);
    //       if (parent) {
    //         parent.replies = parent.replies || [];
    //         parent.replies.push(newComment);
    //       }
    //     } else {
    //       this.comments.push(newComment);
    //     }
    //   },
    //   error: () => {
    //     this.errorMessage = 'Failed to add comment.';
    //     this.isSubmitting = false;
    //   }
    // });
  }

  private findComment(list: Comment[], id: string): Comment | null {
    for (const c of list) {
      if (c.id === id) return c;
      if (c.replies) {
        const found = this.findComment(c.replies, id);
        if (found) return found;
      }
    }
    return null;
  }
}
