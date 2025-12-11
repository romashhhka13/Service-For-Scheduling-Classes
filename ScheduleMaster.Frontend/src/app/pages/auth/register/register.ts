import { Component, inject, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    RouterModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Register {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  faculties = [
    'Автоматики и вычислительной техники',
    'Геологии и геофизики нефти и газа',
    'Гуманитарного образования',
    'Инженерной механики',
    'Комплексной безопасности топливно-энергетического комплекса',
    'Международного энергетического бизнеса',
    'Научно-педагогических кадров и кадров высшей квалификации',
    'Проектирования, сооружения и эксплуатации систем трубопроводного транспорта',
    'Разработки нефтяных и газовых месторождений',
    'Учебно-научный центр довузовской подготовки',
    'Химической технологии и экологии',
    'Экономики и управления',
    'Юридический',
    'филиал РГУ нефти и газа (НИУ) имени И.М. Губкина в г. Атырау (Республика Казахстан)',
    'филиал РГУ нефти и газа (НИУ) имени И.М. Губкина в г. Оренбурге',
    'филиал РГУ нефти и газа (НИУ) имени И.М. Губкина в г. Ташкенте (Республика Узбекистан)',
    'Дополнительное образование'
  ];

  form = this.fb.group({
    email: ['',
      [
        Validators.required,
        Validators.email,
        Validators.pattern(/^[^@\s]+@[^@\s]+\.[^@\s]+$/)
      ]
    ],
    password: ['',
      [
        Validators.required,
        Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#]{8,}$/)
      ]
    ],
    confirmPassword: ['', [Validators.required]],
    surname: ['', [Validators.required]],
    name: ['', [Validators.required]],
    middleName: [''],
    faculty: ['', [Validators.required]],
    groupName: ['',
      [
        Validators.required,
        Validators.pattern(/^[А-ЯЁа-яё]{2,3}-\d{2}-\d{2}$/)
      ]
    ]
  });

  errorMessage = '';
  showServerError = false;
  loading = false;

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.form.value.password !== this.form.value.confirmPassword) {
      this.errorMessage = 'Пароли не совпадают';
      this.showServerError = true;
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.showServerError = false;
    this.cdr.markForCheck();

    const { email, password, surname, name, middleName, faculty, groupName } = this.form.value;

    this.auth.register({
      email: email!,
      password: password!,
      surname: surname!,
      name: name!,
      middleName: middleName || undefined,
      role: 'user',
      faculty: faculty!,
      groupName: groupName!
    }).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error('Register error', err);
        this.loading = false;
        this.errorMessage = err.error?.message || 'Ошибка при регистрации';
        this.showServerError = true;
        this.cdr.markForCheck();
      }
    });
  }
}
