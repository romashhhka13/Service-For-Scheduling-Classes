import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StudioResponseDTO {
    id: string;
    title: string;
    categoryId?: string;
    memberCount: number;
    isLeader: boolean;
}

export interface StudioWithRole extends StudioResponseDTO {
    isLeader: boolean;
}


@Injectable({ providedIn: 'root' })
export class UserService {
    private http = inject(HttpClient);
    private apiUrl = 'http://localhost:5003/api/user';

    getStudiosAsLeader(): Observable<StudioResponseDTO[]> {
        return this.http.get<StudioResponseDTO[]>(
            `${this.apiUrl}/studios_as_leader`,
            { withCredentials: true }
        );
    }

    getStudiosAsMember(): Observable<StudioResponseDTO[]> {
        return this.http.get<StudioResponseDTO[]>(
            `${this.apiUrl}/studios_as_member`,
            { withCredentials: true }
        );
    }
}
