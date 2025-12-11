import { Component, signal, OnInit, inject } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { Header } from './shared/header/header';
import { Footer } from './shared/footer/footer';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('ScheduleMaster.Frontend');

  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    this.authService.checkAuth().subscribe(isAuth => {
      console.log('Пользователь:', (isAuth ? 'авторизован' : 'не авторизован'));

      const currentUrl = this.router.url;
      if (!isAuth && !this.isPublicRoute(currentUrl)) {
        this.router.navigate(['/login']);
      }
      else if (isAuth && this.isPublicRoute(currentUrl)) {
        this.router.navigate(['/home']);
      }
    });
  }

  private isPublicRoute(url: string): boolean {
    const publicRoutes = ['/login', '/register'];
    return publicRoutes.some(route => url.startsWith(route));
  }
}
