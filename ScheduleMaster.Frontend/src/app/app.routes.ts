import { Routes } from '@angular/router';
import { Login } from './pages/auth/login/login';
import { Register } from './pages/auth/register/register';
import { Home } from './pages/dashboard/home/home';
import { Studio } from './pages/dashboard/studio/studio';
import { StudioDetail } from './pages/dashboard/studio-detail/studio-detail';

export const routes: Routes = [
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'home', component: Home },
    { path: 'studio', component: Studio },
    { path: 'studios/:id', component: StudioDetail },
    // Расписание
    // Участники студии
    // Настройки пользователя, редактирование личеного кабинета
    { path: '**', redirectTo: '/login' }

];
