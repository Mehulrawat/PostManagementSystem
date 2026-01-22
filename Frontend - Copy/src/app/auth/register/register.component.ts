import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
    standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink], 
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  form = this.fb.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {}

  submit() {
    if (this.form.invalid) return;
    const { username, email, password } = this.form.value;

    this.errorMessage = '';
    this.successMessage = '';
    this.isSubmitting = true;

    this.auth.register(username!, email!, password!).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage = 'Registered successfully. You can now log in.';
        setTimeout(() => this.router.navigate(['/login']), 1000);
      },
      error: err => {
        this.isSubmitting = false;
        this.errorMessage =
          err?.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
}
