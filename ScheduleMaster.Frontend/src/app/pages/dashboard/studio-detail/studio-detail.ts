import {
    Component,
    ChangeDetectionStrategy,
    OnInit,
    inject,
    signal,
    computed,
    ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';

import {
    StudioService,
    StudioDetailDTO,
    UserResponseDTO,
    StudioCategoryDTO
} from '../../../services/studio.service';
import { UserService, StudioWithRole } from '../../../services/user.service';
import { StudioEditDialog, StudioDialogData } from '../../../dialogs/studio-dialog/studio-dialog';

@Component({
    selector: 'app-studio-detail',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        RouterLink,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatTableModule,
        MatProgressSpinnerModule,
        MatSnackBarModule,
        MatDialogModule,
        MatTooltipModule
    ],
    templateUrl: './studio-detail.html',
    styleUrl: './studio-detail.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class StudioDetail implements OnInit {
    private studioService = inject(StudioService);
    private userService = inject(UserService);
    private route = inject(ActivatedRoute);
    private snackBar = inject(MatSnackBar);
    private dialog = inject(MatDialog);
    private cdr = inject(ChangeDetectorRef);

    studio = signal<StudioDetailDTO | null>(null);
    members = signal<UserResponseDTO[]>([]);
    categories = signal<StudioCategoryDTO[]>([]);
    isLeader = signal(false);
    loading = signal(false);

    searchQuery = signal('');
    facultyFilter = signal<string>('');
    roleFilter = signal<'all' | 'leader' | 'member'>('all');

    viewMode = signal<'cards' | 'table'>('cards');

    filteredMembers = computed(() => {
        let result = this.members();

        const search = this.searchQuery().toLowerCase();
        if (search) {
            result = result.filter(m =>
                `${m.surname} ${m.name}`.toLowerCase().includes(search) ||
                (m.faculty || '').toLowerCase().includes(search) ||
                (m.studyGroup || '').toLowerCase().includes(search)
            );
        }

        if (this.facultyFilter()) {
            result = result.filter(m => (m.faculty || '') === this.facultyFilter());
        }

        const role = this.roleFilter();
        if (role === 'leader') {
            result = result.filter(m => m.isLeader === true);
        } else if (role === 'member') {
            result = result.filter(m => m.isLeader === false);
        }

        return result.sort((a, b) => {
            if (a.isLeader && !b.isLeader) return -1;
            if (!a.isLeader && b.isLeader) return 1;
            return `${a.surname} ${a.name}`.localeCompare(`${b.surname} ${b.name}`);
        });
    });



    faculties = computed(() => {
        const facs = new Set(this.members().map(m => m.faculty || ''));
        return Array.from(facs).filter(f => f !== '').sort();
    });


    studyGroups = computed(() => {
        const sgs = new Set(this.members().map(m => m.studyGroup || ''));
        return Array.from(sgs).filter(g => g !== '').sort();
    });


    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadData(id);
        }
    }

    private loadData(studioId: string): void {
        this.loading.set(true);

        const studio$ = this.studioService.getStudioById(studioId);
        const members$ = this.studioService.getStudioUsers(studioId);
        const categories$ = this.studioService.getCategories();
        const leaderStudios$ = this.userService.getStudiosAsLeader(); // уже есть в списке студий

        import('rxjs').then(({ forkJoin }) => {
            forkJoin({
                studio: studio$,
                members: members$,
                categories: categories$,
                leaderStudios: leaderStudios$
            }).subscribe({
                next: ({ studio, members, categories, leaderStudios }) => {
                    this.studio.set(studio);
                    this.members.set(members);
                    this.categories.set(categories);

                    // проверяем, есть ли текущая студия в списке «где я лидер»
                    const isLeaderHere = (leaderStudios as StudioWithRole[])
                        .some(s => s.id === studio.id);
                    this.isLeader.set(isLeaderHere);

                    this.loading.set(false);
                    this.cdr.markForCheck();
                },
                error: (err) => {
                    console.error('❌ Ошибка загрузки студии:', err);
                    this.snackBar.open('Ошибка загрузки студии', 'Закрыть', { duration: 3000 });
                    this.loading.set(false);
                    this.cdr.markForCheck();
                }
            });
        });
    }

    openEditDialog(): void {
        const s = this.studio();
        if (!s || !this.isLeader()) {
            this.snackBar.open('Только руководитель может редактировать студию', 'Закрыть', { duration: 2000 });
            return;
        }

        const data: StudioDialogData = {
            categories: this.categories(),
            studio: {
                id: s.id,
                title: s.title,
                categoryId: s.categoryId
            }
        };

        const dialogRef = this.dialog.open(StudioEditDialog, {
            width: '500px',
            disableClose: false,
            data
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.studioService.updateStudio(s.id, result).subscribe({
                    next: () => {
                        this.snackBar.open('Студия обновлена ✅', 'Закрыть', { duration: 2000 });
                        this.studio.update(prev => prev ? { ...prev, ...result } : prev);
                    },
                    error: err => {
                        console.error('❌ Ошибка обновления:', err);
                        this.snackBar.open('Ошибка обновления студии', 'Закрыть', { duration: 3000 });
                    }
                });
            }
        });
    }

    getCategoryName(categoryId?: string): string {
        if (!categoryId) return '—';
        const cat = this.categories().find(c => c.id === categoryId);
        return cat ? cat.category : '—';
    }
}
