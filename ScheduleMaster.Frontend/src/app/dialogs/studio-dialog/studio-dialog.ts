import { Component, Inject, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

import { StudioCategoryDTO, StudioEditData } from '../../services/studio.service';

export interface StudioDialogData {
  categories: StudioCategoryDTO[];
  studio?: StudioEditData; // если есть → редактирование, если нет → создание
}

@Component({
  selector: 'app-studio-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>
      {{ data.studio ? 'Редактировать студию' : 'Создать новую студию' }}
    </h2>

    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Название студии *</mat-label>
          <input 
            matInput 
            formControlName="title" 
            placeholder="Например: Frontend Club"
            autofocus
          >
          <mat-error *ngIf="form.get('title')?.hasError('required')">
            Название обязательно
          </mat-error>
          <mat-error *ngIf="form.get('title')?.hasError('minlength')">
            Минимум 2 символа
          </mat-error>
          <mat-error *ngIf="form.get('title')?.hasError('maxlength')">
            Максимум 100 символов
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Категория (опционально)</mat-label>
          <mat-select formControlName="categoryId">
            <mat-option [value]="null">Без категории</mat-option>
            <mat-option *ngFor="let cat of data.categories" [value]="cat.id">
              {{ cat.category }}
            </mat-option>
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end" class="dialog-actions">
      <button mat-button (click)="onCancel()">Отмена</button>
      <button 
        mat-raised-button 
        color="primary"
        (click)="onSubmit()"
        [disabled]="form.invalid"
      >
        {{ data.studio ? 'Сохранить' : 'Создать' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      min-width: 400px;
      padding-top: 16px;
    }
    .full-width {
      width: 100%;
    }
    .dialog-actions {
      display: flex;
      gap: 8px;
      padding-top: 16px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StudioEditDialog implements OnInit {
  private fb = inject(FormBuilder);

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    categoryId: ['']
  });

  constructor(
    public dialogRef: MatDialogRef<StudioEditDialog>,
    @Inject(MAT_DIALOG_DATA) public data: StudioDialogData
  ) { }

  ngOnInit(): void {
    // если редактирование — проставляем начальные значения
    if (this.data.studio) {
      this.form.patchValue({
        title: this.data.studio.title,
        categoryId: this.data.studio.categoryId ?? ''
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const result = {
      title: this.form.value.title!,
      categoryId: this.form.value.categoryId || undefined
    };

    this.dialogRef.close(result);
  }
}
