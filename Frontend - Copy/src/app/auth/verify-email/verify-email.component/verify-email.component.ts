import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import{ AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html'
})
export class VerifyEmailComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService
  ) {}
  
  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) return;
  
    this.auth.verifyEmail(token).subscribe({
      next: () => alert('Email verified successfully'),
      error: () => alert('Invalid or expired link')
    });
  }
}