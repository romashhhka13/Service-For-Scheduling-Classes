import { Injectable, inject, signal } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { Observable, of } from 'rxjs'
import { tap, map, catchError } from 'rxjs/operators'

interface LoginRequest {
    email: string;
    password: string;
}

interface RegisterRequest {
    email: string;
    password: string;
    surname: string;
    name: string;
    middleName?: string;
    role?: string;
    faculty?: string;
    groupName?: string;
}

interface LoginResponse {
    userId: string;
    email: string;
    role: string;
    token: string;
    expiresIn: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);
    private router = inject(Router);
    private apiUrl = 'http://localhost:5003/api';

    readonly isAuthenticated = signal(false);

    login(body: { email: string; password: string }): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/login`, body, {
            withCredentials: true
        }).pipe(
            tap(() => {
                this.isAuthenticated.set(true);
            })
        );
    }

    register(body: RegisterRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/auth/register`, body, {
            withCredentials: true
        }).pipe(
            tap(() => {
                this.isAuthenticated.set(true);
                return true;
            })
        );
    }

    checkAuth(): Observable<boolean> {
        return this.http.get(`${this.apiUrl}/auth/check`, {
            withCredentials: true
        }).pipe(
            map(() => {
                this.isAuthenticated.set(true);
                return true;
            }),
            catchError(() => {
                this.isAuthenticated.set(false);
                return of(false);
            })
        );
    }

    logout() {
        this.http.post(`${this.apiUrl}/auth/logout`, {}, {
            withCredentials: true
        }).subscribe({
            next: () => {
                this.isAuthenticated.set(false);
                this.router.navigate(['/login']);
            },
            error: () => {
                this.isAuthenticated.set(false);
                this.router.navigate(['/login']);
            }
        });
    }

}
