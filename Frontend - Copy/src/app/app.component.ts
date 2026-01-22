import { Component } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { RouterOutlet } from '@angular/router';
import { CommonModule} from '@angular/common';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';

@Component({

  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,CommonModule,RouterLink], 

  templateUrl: './app.component.html'

})
export class AppComponent {


  constructor(public auth: AuthService, private router: Router) {
    
  }

  get avatarInitials(): string {
    const name = this.auth.getUsername?.() || '';
    const parts = name.trim().split(/\s+/);
    const initials = parts.slice(0, 2).map(p => p[0]?.toUpperCase() || '').join('');
    return initials || 'U';
  }


isProfilePanelOpen = false;

toggleProfilePanel() {
  this.isProfilePanelOpen = !this.isProfilePanelOpen;
  
}

closeProfilePanel() {
  if (this.isProfilePanelOpen) {
    this.isProfilePanelOpen = false;
  }
}


  logout() {
    this.auth.logout();
    this.isProfilePanelOpen = false;
     const tree=this.router.createUrlTree(['/']);
     this.router.navigateByUrl(tree)
  }
}















 