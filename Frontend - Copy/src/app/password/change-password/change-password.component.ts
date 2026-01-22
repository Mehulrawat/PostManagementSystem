
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { PasswordService } from '../../core/services/password.service';
import { Router } from '@angular/router';
 import { AuthService } from '../../core/services/auth.service'; 

function matchOtherValidator(otherControlName: string) {
  return (control: AbstractControl): ValidationErrors | null => {
    const parent = control.parent;
    if (!parent) return null;
    const other = parent.get(otherControlName);
    if (!other) return null;
    return control.value === other.value ? null : { mismatch: true };
  };
}

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent {
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  form = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required]],
    confirmNewPassword: ['', [Validators.required, matchOtherValidator('newPassword')]],
  });

  constructor(
    private fb: FormBuilder,
    private passwordSvc: PasswordService,
    private router: Router,
     private auth: AuthService 
  ) {}

  get f() { return this.form.controls; }

  submit() {
    this.successMessage = '';
    this.errorMessage = '';
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSubmitting = true;

    const payload = {
      currentPassword: this.f.currentPassword.value!,
      newPassword: this.f.newPassword.value!,
    };

    this.passwordSvc.changePassword(payload).subscribe({
      next: () => {
        this.isSubmitting = false;

      
        this.auth.logout();

        // Redirect to login page
        this.router.navigate(['/login']);

        
        this.successMessage = 'Password changed successfully. Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 500);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message ?? 'Failed to change password.';
      }
    });
  }
}
