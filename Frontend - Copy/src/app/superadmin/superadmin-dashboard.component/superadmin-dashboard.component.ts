import { Component, OnInit } from '@angular/core';
import { Post, PostStatus } from '../../core/models/post.model';
import { PostService } from '../../core/services/post.service';
import { SuperAdminService } from '../../core/services/superadmin.service';
import { AdminStatus,Users } from '../../core/models/admin.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import { UserRole } from 'src/app/core/models/user.model';

@Component({
  selector: 'app-superadmin-dashboard',
  imports:[CommonModule],
  templateUrl: './superadmin-dashboard.component.html'
})
export class SuperAdminDashboardComponent implements OnInit {
  users: Users[] = [];
  isLoading = false;
  errorMessage = '';

  adminStauts = AdminStatus;

  constructor(private SuperAdminservice: SuperAdminService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.isLoading = true;
    this.SuperAdminservice.getAllUsers().subscribe({
      next:users => {
        this.users = users;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load Users/Admins.';
        this.isLoading = false;
      }
    });
  }

    statusLabel(status: AdminStatus) {
    return AdminStatus[status];
  }


  
//  private normalizeRoles(roles: (UserRole | string)[] | undefined | null): string[] {
//     return (roles ?? []).map(r => (typeof r === 'string' ? r : r.toString())).map(r => r.trim());
//   }

normalizeRoles(roles: UserRole[] | string[]): string[] {
  // convert to string array
  return roles.map(r => typeof r === 'string' ? r : r.toString());
}
hasRole(users: Users | undefined | null, role: UserRole | string): boolean {
  if (!users) return false;

  // Normalize roles to string[]
  const roles: string[] = this.normalizeRoles(users.roles);

  // Always convert role to string for comparison
  return roles.includes(typeof role === "string" ? role : String(role));
}


 isAdmin(users:Users | undefined | null): boolean {
    if (!users) return false;
    const roles = this.normalizeRoles(users.roles);
    // Treat Admin and SuperAdmin as admin-tier
    return roles.includes(UserRole.Admin);
  }



isNormalUser(user:Users | undefined | null): boolean {
    if (!user) return false;
    const roles = this.normalizeRoles(user.roles);
    // "Normal user" means the user has 'User' and does NOT have admin-tier
    return roles.includes(UserRole.User) && !this.isAdmin(user);
  }


  suspend(users: Users) {
    this.SuperAdminservice.suspendUser(users.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to suspend the user/admin.')
    });
  }
    reinstate(users: Users) {
    this.SuperAdminservice.reinstateUser(users.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to reinstate the user/admin.')
    });
  }
   delete(users: Users) {
    this.SuperAdminservice.deleteuser(users.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to delete user/admin.')
    });
  }

    promote(users: Users) {
    this.SuperAdminservice.promoteUser(users.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to promote admin.')
    });
  }
 demote(users: Users) {
    this.SuperAdminservice.demoteUser(users.id).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'Failed to demote admin.')
    });
  }

 
}
