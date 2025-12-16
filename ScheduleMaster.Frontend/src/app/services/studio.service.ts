import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface StudioCategoryDTO {
    id: string;
    category: string;
}


export interface StudioEditData {
    id: string;
    title: string;
    categoryId?: string;
}


export interface UserResponseDTO {
    id: string;
    surname: string;
    name: string;
    middleName?: string;
    email: string;
    role: string;
    faculty?: string;
    studyGroup?: string;
    isLeader: boolean;
}

export interface StudioGroupDTO {
    id: string;
    title: string;
}

export interface StudioDetailDTO {
    id: string;
    title: string;
    categoryId?: string;
    memberCount: number;
}


@Injectable({ providedIn: 'root' })
export class StudioService {
    private http = inject(HttpClient);
    private apiUrl = 'http://localhost:5003/api/studio';

    createStudio(dto: { title: string; categoryId?: string }): Observable<string> {
        return this.http.post<string>(this.apiUrl, dto, {
            withCredentials: true
        });
    }

    updateStudio(id: string, dto: { title?: string; categoryId?: string }): Observable<void> {
        return this.http.put<void>(
            `${this.apiUrl}/${id}`, dto,
            { withCredentials: true }
        );
    }

    deleteStudio(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`, {
            withCredentials: true
        });
    }

    getCategories(): Observable<StudioCategoryDTO[]> {
        return this.http.get<StudioCategoryDTO[]>(
            `${this.apiUrl}/categories`,
            { withCredentials: true }
        );
    }

    getStudioById(id: string): Observable<StudioDetailDTO> {
        return this.http.get<StudioDetailDTO>(
            `${this.apiUrl}/${id}`,
            { withCredentials: true }
        );
    }

    getStudioUsers(studioId: string): Observable<UserResponseDTO[]> {
        return this.http.get<UserResponseDTO[]>(
            `${this.apiUrl}/${studioId}/users`,
            { withCredentials: true }
        );
    }

    // getStudioGroups(studioId: string): Observable<StudioGroupDTO[]> {
    //     return this.http.get<StudioGroupDTO[]>(
    //         `${this.apiUrl}/${studioId}/groups`,
    //         { withCredentials: true }
    //     );
    // }
}
