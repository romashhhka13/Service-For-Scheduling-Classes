import { Component, inject, ChangeDetectionStrategy, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    RouterModule
  ],
  templateUrl: './header.html',
  styleUrl: './header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Header {
  private router = inject(Router);
  private auth = inject(AuthService);

  // auth‑страницы
  readonly isAuthRoute = computed(() => {
    const url = this.router.url;
    return url.startsWith('/login') || url.startsWith('/register');
  });

  // показывать меню?
  readonly showMenu = computed(() => this.auth.isAuthenticated() && !this.isAuthRoute());

  onLogout() {
    this.auth.logout();
  }
}
