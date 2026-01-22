

import { Component, OnInit } from '@angular/core';
import { UserService } from '../../core/services/user.service';
import { Users } from '../../core/models/user.model';
import { RouterLink, RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-user-profile',
  imports:[RouterLink,DatePipe],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
  standalone: true
})
export class UserProfileComponent implements OnInit {
  user!: Users;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    const userId = '123'; // or get from route params
    this.userService.getUserById(userId).subscribe(res => {
      this.user = res;
    });
  }
}


