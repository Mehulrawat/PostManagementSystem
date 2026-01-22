// import { Component } from '@angular/core';
// import { FormBuilder, Validators } from '@angular/forms';
// import { Router, ActivatedRoute } from '@angular/router';
// import { AuthService } from '../../core/services/auth.service';
// import { ReactiveFormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-login',
  
//   standalone: true,
//   imports: [CommonModule, ReactiveFormsModule],

//   templateUrl: './login.component.html'
// })
// export class LoginComponent {
//   errorMessage = '';
//   isSubmitting = false;

//   form = this.fb.group({
//     username: ['', Validators.required],
//     password: ['', Validators.required]
//   });

//   returnUrl: string = '/posts';

//   constructor(
//     private fb: FormBuilder,
//     private auth: AuthService,
//     private router: Router,
//     private route: ActivatedRoute
//   ) {
//     this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';
//   }

//   submit() {
//     if (this.form.invalid) return;

//     const { username, password } = this.form.value;
//     this.errorMessage = '';
//     this.isSubmitting = true;

//     this.auth.login(username!, password!).subscribe({
//       next: () => {
//         this.isSubmitting = false;
//         this.router.navigateByUrl(this.returnUrl);
//       },
//       error: err => {
//         this.isSubmitting = false;
//         this.errorMessage =
//           err?.error?.message || 'Login failed. Please check your credentials.';
//       }
//     });
//   }
// }



// src/app/auth/login/login.component.ts
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service'; // adjust path

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  // Build form
  form = this.fb.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  isSubmitting = false;
  errorMessage: string | null = null;

  submit() {
    if (this.form.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = null;

    const { username, password } = this.form.value;

    this.auth.login(username!, password!).subscribe({
      next: () => {
        //  Navigate to posts (or wherever you want after login)
        this.router.navigate(['/posts']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage =
          err?.error?.message ??
          'Login failed. Please check your credentials and try again.';
      }
    });
  }
}
