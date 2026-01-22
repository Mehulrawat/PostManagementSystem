import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { PostListComponent } from './posts/post-list/post-list.component';
import { MyPostsComponent } from './posts/my-posts/my-posts.component';
import { PostDetailComponent } from './posts/post-detail/post-detail.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';
import { SuperAdminDashboardComponent } from './superadmin/superadmin-dashboard.component/superadmin-dashboard.component';
import { PostCreateComponent } from './posts/post-create/post-create.component';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { SuperAdminGuard } from './core/guards/superadmin.guard-guard';
import { ChangePasswordComponent } from './password/change-password/change-password.component';
import { UserProfileComponent } from './profile/profile.component/profile.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'posts', component: PostListComponent, canActivate: [AuthGuard] },
  {
    path: 'posts/create',
    component: PostCreateComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'posts/my',
    component: MyPostsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'posts/:id',
    component: PostDetailComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [AdminGuard]
  },

{
    path: 'superadmin',
    component: SuperAdminDashboardComponent,
    canActivate: [SuperAdminGuard]
  },

 { path: 'change-password', component: ChangePasswordComponent, canActivate: [AuthGuard] },
//  {path:'/profile', component:UserProfileComponent,canActivate:[AuthGuard]},

  { path: '**', redirectTo: 'posts' }
];

// @NgModule({
//   imports: [RouterModule.forRoot(routes)],
//   exports: [RouterModule]
// })
// export class AppRoutingModule {}



// src/app/app-routing.ts (or app.routes.ts)
// import { Routes } from '@angular/router';

// import { PostListComponent } from './posts/post-list/post-list.component';
// import { PostDetailComponent } from './posts/post-detail/post-detail.component';
// import { LoginComponent } from './auth/login/login.component';
// import { RegisterComponent } from './auth/register/register.component';
// import { MyPostsComponent } from './posts/my-posts/my-posts.component';
// import { PostCreateComponent } from './posts/post-create/post-create.component';
// import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';

// export const routes: Routes = [
//   { path: '', component: PostListComponent },
//   { path: 'posts/:id', component: PostDetailComponent },
//   { path: 'login', component: LoginComponent },
//   { path: 'register', component: RegisterComponent },
//   { path: 'my-posts', component: MyPostsComponent },
//   { path: 'create', component: PostCreateComponent },
//   { path: 'admin', component: AdminDashboardComponent }
// ];
