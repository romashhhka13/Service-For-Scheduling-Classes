import {
  Component,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { StudioService, StudioCategoryDTO } from '../../../services/studio.service';
import { UserService, StudioWithRole } from '../../../services/user.service'
import { StudioEditDialog } from '../../../dialogs/studio-dialog/studio-dialog';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-studios',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatIconModule,
    MatDialogModule,
    RouterLink
  ],
  templateUrl: './studio.html',
  styleUrl: './studio.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Studio implements OnInit {
  private studioService = inject(StudioService);
  private userService = inject(UserService)
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);
  private dialog = inject(MatDialog);

  studios = signal<StudioWithRole[]>([]);
  categories = signal<StudioCategoryDTO[]>([]);
  loading = signal(false);

  createForm = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    categoryId: ['']
  });

  ngOnInit(): void {
    this.loadCategories();
    this.loadStudios();

  }

  getCategory(categoryId?: string | null): string {
    if (!categoryId) {
      return '—';
    }

    const cats = this.categories();
    const found = cats.find(c => c.id === categoryId);
    return found ? found.category : '—';
  }

  loadCategories(): void {
    this.studioService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Ошибка загрузки категорий', err);
        this.snackBar.open('Не удалось загрузить категории', 'Закрыть', { duration: 3000 });
      }
    });
  }

  loadStudios(): void {
    this.loading.set(true);

    forkJoin({
      leader: this.userService.getStudiosAsLeader(),
      member: this.userService.getStudiosAsMember()
    }).subscribe({
      next: ({ leader, member }) => {
        const leaderStudios: StudioWithRole[] = leader.map(s => ({ ...s, isLeader: true }));
        const memberStudios: StudioWithRole[] = member.map(s => ({ ...s, isLeader: false }));

        const merged: StudioWithRole[] = [
          ...leaderStudios,
          ...memberStudios.filter(m => !leaderStudios.some(l => l.id === m.id))
        ];

        this.studios.set(merged);
        this.loading.set(false);
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Ошибка загрузки студий', err);
        this.snackBar.open('Ошибка загрузки студий', 'Закрыть', { duration: 3000 });
        this.loading.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(StudioEditDialog, {
      width: '500px',
      disableClose: false,
      data: {
        categories: this.categories()
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.createStudio(result);
      }
    });
  }

  createStudio(dto: { title: string; categoryId?: string }): void {
    this.studioService.createStudio(dto).subscribe({
      next: () => {
        this.snackBar.open('Студия создана', 'Закрыть', { duration: 2000 });
        this.loadStudios();
      },
      error: (err) => {
        console.error('Ошибка создания студии', err);
        this.snackBar.open('Ошибка создания студии', 'Закрыть', { duration: 3000 });
      }
    });
  }

  deleteStudio(id: string, event?: MouseEvent): void {
    if (event) {
      event.stopPropagation();
    }

    if (!confirm('Удалить эту студию?')) {
      return;
    }

    this.studioService.deleteStudio(id).subscribe({
      next: () => {
        this.snackBar.open('Студия удалена', 'Закрыть', { duration: 2000 });
        this.loadStudios();
      },
      error: (err) => {
        console.error('Ошибка удаления студии', err);
        this.snackBar.open('Ошибка удаления студии', 'Закрыть', { duration: 3000 });
      }
    });
  }
}
